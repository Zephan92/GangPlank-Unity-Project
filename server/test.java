import java.io.*;
import java.net.*;

public class test{
	public static void main(String args[]) throws Exception{
		Socket socket = new Socket("127.0.0.1",args.length>0?Integer.parseInt(args[0]):5555);
		BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
		PrintWriter out = new PrintWriter(socket.getOutputStream(), true);

		out.print("abc");
		out.flush();
		String read;
		while((read = in.readLine()) != null){
			System.out.println("in:"+read);
		}

		socket.close();
	}
}
