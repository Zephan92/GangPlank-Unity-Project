import java.io.*;
import java.net.*;
import java.nio.channels.*;
//import java.util.*;
//import java.util.stream.*;
//import java.util.concurrent.*;

import static java.lang.System.out;

public class MatchServer{

	AsynchronousServerSocketChannel listener;

	public MatchServer(){}

	public void start(int port){
		try{
			listener = AsynchronousServerSocketChannel.open().bind(new InetSocketAddress(port));
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
		try{
			out.println(ch.getRemoteAddress());
		}catch(IOException exc){
			exc.printStackTrace(out);
		}
	}

	public void stop(){
		try{
			listener.close();
		}catch(IOException exc){}
	}

	public static void main(String[] args) throws InterruptedException{
		final MatchServer server = new MatchServer();
		server.start(args.length > 0? Integer.parseInt(args[0]) : 5555);

		Runtime.getRuntime().addShutdownHook(new Thread(){
			public void run(){
				server.stop();
			}
		});

		while(true){
			Thread.sleep(1000);
		}
	}
}
