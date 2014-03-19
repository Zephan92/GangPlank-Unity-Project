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
	private void addHost(User user, String msg){
		String[] split = msg.split(UNIT_SPLIT_STR);
		if(split.length < 2){
			user.send(tooFewArgs(split[0]));
			return;
		}

		if(hosts.containsKey(split[1])){
			user.send(composeMessage("fail","username taken"));
		}
		else{
			if(user.getName().equals(User.DEFAULT_NAME)) user.setName(split[1]);
			hosts.put(split[1], user);
			user.send(composeMessage("ok","added user '"+split[1]+"'"));
		}
	}

	private void listHosts(User user, String msg){
		String[] split = msg.split(UNIT_SPLIT_STR);
		int num = split.length < 2 || split[1].equals("all") ? hosts.size() : Math.min(hosts.size(), Integer.parseInt(split[1]));
		if(num == 0){
			user.send(composeMessage("fail","no hosts"));
		}
		else{
			user.send(composeMessage(Stream.concat(Stream.of("ok"),hosts.keySet().stream().limit(num)).toArray(String[]::new)));
		}
	}

	private String tooFewArgs(String cmd){
		return composeMessage("err","too few args for '"+cmd+"'");
	}

	private String composeUnits(String... units){
		return Arrays.stream(units).collect(Collectors.joining(UNIT_SPLIT_STR));
	}
	private String composeMessageMulti(String... records){
		return Arrays.stream(records).collect(Collectors.joining(RECORD_SPLIT_STR))+END_TRANSMISSION;
	}
	private String composeMessage(String... units){
		return composeMessageMulti(composeUnits(units));
	}

	private void disconnectUser(User user){
		out.println("Disconnected:"+user);
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

			setName(DEFAULT_NAME);
		}

		public String getName(){return name;}
		public void setName(String n){
			name = n;
			tostr = name+"@"+address;
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
					out.println("sent{"+s+"}");
				}
				public void failed(Throwable exc, String s){
					out.println("failed to send{"+s+"} because "+exc);
				}
			});
			*/
		}

		public String toString(){
			return tostr;
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
