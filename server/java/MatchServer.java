import java.io.*;
import java.net.*;
import java.nio.*;
import java.nio.channels.*;
import java.nio.charset.Charset;
import java.util.*;
import java.util.stream.*;
import java.util.function.*;
import java.util.concurrent.*;

import static java.lang.System.out;

public class MatchServer{

	private static final char END_TRANSMISSION = 0x17, UNIT_SPLIT = 0x1F, RECORD_SPLIT = 0x1E;
	private static final String END_TRANSMISSION_STR = ""+END_TRANSMISSION, UNIT_SPLIT_STR = ""+UNIT_SPLIT, RECORD_SPLIT_STR = ""+RECORD_SPLIT;
	private static final Charset ASCII = Charset.forName("US-ASCII");

	AsynchronousServerSocketChannel listener;

	@FunctionalInterface
	private interface MessageHandler{
		void handle(User u, String s);
	}

	private HashMap<String, MessageHandler> messageHandlers;
	public MatchServer(){
		messageHandlers = new HashMap<String, MessageHandler>();
		messageHandlers.put("host", this::addHost);
		messageHandlers.put("list_hosts", this::listHosts);
		messageHandlers.put("join", this::joinGroup);
		messageHandlers.put("list_group", this::listGroup);
		messageHandlers.put("group", this::sendToGroup);
	}

	public void stop(){
		try{
			listener.close();
		}catch(IOException exc){}
	}

	public void start(int port){
		try{
			listener = AsynchronousServerSocketChannel.open().bind(new InetSocketAddress(port));
			out.println("Listening for connections...");
			listener.accept(null, new CompletionHandler<AsynchronousSocketChannel,Void>() {
				public void completed(AsynchronousSocketChannel ch, Void att) {
					listener.accept(null, this);

					handleConnection(ch);
				}
				public void failed(Throwable exc, Void att) {
					exc.printStackTrace(out);
				}
			});
		}catch(IOException exc){
			out.println("Failed to establish socket, exiting");
			System.exit(1);
		}
	}

	private void handleConnection(AsynchronousSocketChannel ch){
		User user = new User(ch);
		out.println("Connected:"+user);
		ch.read(user.buffer, user, new CompletionHandler<Integer, User>(){
			public void completed(Integer bytesRead, User user){
				if(user.consumeBuffer() == END_TRANSMISSION){
					handleMessage(user, user.consumeMessage());
				}

				if(bytesRead >= 0){
					user.channel.read(user.buffer, user, this);
				}
				else{
					disconnectUser(user);
				}
			}
			public void failed(Throwable exc, User user){
				disconnectUser(user);
			}
		});
	}
	private void handleMessage(User user, String msg){
		int endCmd = msg.indexOf(UNIT_SPLIT);
		String cmd = endCmd>0?msg.substring(0, endCmd):msg;
		messageHandlers.getOrDefault(cmd, this::commandNotFound).handle(user, msg);
	}

	private void commandNotFound(User user, String msg){
		user.send(composeMessage("err","unknown command '"+msg+"'"));		
	}

	private ConcurrentHashMap<String, User> hosts = new ConcurrentHashMap<String, User>();
	private ConcurrentHashMap<User, Group> users = new ConcurrentHashMap<User, Group>();

	private static final String FAIL_NAME_TAKEN = composeMessage("fail","user name taken"),
	                            FAIL_NO_HOSTS = composeMessage("fail","no hosts"),
	                            FAIL_HOST_NOT_FOUND = composeMessage("fail","host not found"),
										 FAIL_NOT_IN_GROUP = composeMessage("fail","not in a group"),
										 OK_JOINED_GROUP = composeMessage("ok","joined group"),
										 ERR_NAME_MISMATCH = composeMessage("err","name mismatch detected");

	private void addHost(User user, String msg){
		String[] split = msg.split(UNIT_SPLIT_STR);
		if(split.length < 2){
			user.send(tooFewArgs(split[0]));
			return;
		}

		if(hosts.containsKey(split[1])){
			user.send(FAIL_NAME_TAKEN);
		}
		else{
			if(user.getName().equals(User.DEFAULT_NAME)){
				user.setName(split[1]);
			}
			else if(!user.getName().equals(split[1])){
				user.send(ERR_NAME_MISMATCH);
				return;
			}
			hosts.put(split[1], user);
			users.put(user, new Group(user));
			user.send(composeMessage("ok","added user '"+split[1]+"'"));
		}
	}

	private void listHosts(User user, String msg){
		String[] split = msg.split(UNIT_SPLIT_STR);
		int num = split.length < 2 || split[1].equals("all") ? hosts.size() : Math.min(hosts.size(), Integer.parseInt(split[1]));
		if(num == 0){
			user.send(FAIL_NO_HOSTS);
		}
		else{
			user.send(composeMessage("ok",hosts.keySet().stream().limit(num).toArray(String[]::new)));
		}
	}

	private void joinGroup(User user, String msg){
		String[] split = msg.split(UNIT_SPLIT_STR);
		if(split.length < 3){
			user.send(tooFewArgs(split[0]));
			return;
		}

		User host = hosts.get(split[1]);
		if(host == null){
			user.send(FAIL_HOST_NOT_FOUND);
			return;
		}
		
		Group group = users.get(host);
		if(group.members.containsKey(split[2])){
			user.send(FAIL_NAME_TAKEN);
			return;
		}

		if(user.getName().equals(User.DEFAULT_NAME)){
			user.setName(split[2]);
		}
		else if(!user.getName().equals(split[2])){
			user.send(ERR_NAME_MISMATCH);
			return;
		}
		users.put(user, group);
		user.send(OK_JOINED_GROUP);
		group.send(composeMessage("hook","joined",user.getName()));
		group.add(user);
	}

	private void listGroup(User user, String msg){
		Group group = users.get(user);
		if(group == null){
			user.send(FAIL_NOT_IN_GROUP);
			return;
		}
		
		user.send(composeMessage("ok", group.members.keySet().stream().toArray(String[]::new)));
	}

	private void sendToGroup(User user, String msg){
		Group group = users.get(user);
		if(group == null){
			user.send(FAIL_NOT_IN_GROUP);
		}

		group.send(composeMessage("hook",msg.substring(msg.indexOf(UNIT_SPLIT)+1)));
	}

	private static String tooFewArgs(String cmd){
		return composeMessage("err","too few args for '"+cmd+"'");
	}

	private static String composeUnits(String... units){
		return Arrays.stream(units).collect(Collectors.joining(UNIT_SPLIT_STR));
	}
	private static String composeMessageMulti(String... records){
		return Arrays.stream(records).collect(Collectors.joining(RECORD_SPLIT_STR))+END_TRANSMISSION;
	}
	private static String composeMessage(String... units){
		return composeMessageMulti(composeUnits(units));
	}
	private static String composeMessage(String cmd, String[] args){
		return composeMessageMulti(composeUnits(cmd, composeUnits(args)));
	}

	private void disconnectUser(User user){
		out.println("Disconnected:"+user);
		if(users.containsKey(user)){
			users.get(user).members.remove(user.getName());
			users.remove(user);
		}
		hosts.remove(user.getName());
		try{
			user.channel.close();
		}catch(IOException exc){}
	}

	private class User{
		public static final String DEFAULT_NAME = "anon"+UNIT_SPLIT+"user";
		public final ByteBuffer buffer = ByteBuffer.allocate(10);
		public final AsynchronousSocketChannel channel;
		public final SocketAddress address;

		private final StringBuilder message = new StringBuilder();
		private String name, tostr;

		public User(AsynchronousSocketChannel channel){
			this.channel = channel;

			address = ((Supplier<SocketAddress>)(()->{
				try{
					return channel.getRemoteAddress();
				}catch(IOException exc){
					return null;
				}
			})).get();

			setName(DEFAULT_NAME, false);
		}

		public String getName(){return name;}
		public void setName(String n){
			setName(n, true);
		}
		private void setName(String n, boolean announce){
			name = n;
			tostr = name+"@"+address;
			if(announce) out.println("Id set:"+this);
		}

		public char consumeBuffer(){
			if(buffer.position() <= 0) return 0;
			byte data[] = new byte[buffer.position()];
			((ByteBuffer)(buffer.asReadOnlyBuffer().flip())).get(data);
			message.append(new String(data, ASCII));
			buffer.rewind();
			return (char)data[data.length-1];
		}

		public String consumeMessage(){
			String msg = message.toString();
			message.setLength(0);
			message.trimToSize();
			return msg.substring(0,msg.lastIndexOf(END_TRANSMISSION));
		}

		public void send(String msg){
			channel.write(ByteBuffer.wrap(msg.getBytes(ASCII)));
			/*
			channel.write(ByteBuffer.wrap(msg.getBytes(ASCII)), msg, new CompletionHandler<Integer, String>(){
				public void completed(Integer i, String s){
					out.println("sent to "+getName()+"{"+s+"}");
				}
				public void failed(Throwable exc, String s){
					out.println("failed to send{"+s+"} because "+exc);
				}
			});
			//*/
		}

		public String toString(){
			return tostr;
		}

		public int hashCode(){
			return tostr.hashCode();
		}
	}

	private class Group{
		public final ConcurrentHashMap<String, User> members = new ConcurrentHashMap<String, User>();
		public Group(User... users){
			for(User u:users){
				add(u);
			}
		}
		public void add(User u){
			members.put(u.getName(), u);
		}

		public void send(String msg){
			members.values().stream().forEach(user -> user.send(msg));
		}
	}

	public static void main(String[] args) throws InterruptedException{
		final MatchServer server = new MatchServer();
		Runtime.getRuntime().addShutdownHook(new Thread(()-> server.stop()));
		server.start(args.length > 0? Integer.parseInt(args[0]) : 5555);

		while(true){
			Thread.sleep(1000);
		}
	}
}
