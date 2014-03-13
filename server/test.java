import java.util.*;
import java.io.*;
import java.net.*;

import static java.lang.System.out;

public class test{

	static String ip_addr = "54.200.46.105";
	public static void main(String args[]) throws Exception{
		Scanner scan = new Scanner(System.in);
		//*
		Socket socket = new Socket(args.length>1?args[1]:ip_addr,args.length>0?Integer.parseInt(args[0]):5555);
		BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
		PrintWriter out = new PrintWriter(socket.getOutputStream(), true);
//*/

		// \x17 ^W end of message
		// \x1F ^^ split cmd/msg
		// \x1E ^_ split multi message before \x17
		char[] buff = new char[1024];
		while(scan.hasNext()){
			String s = scan.nextLine().replace("\n","")+"\0";
			out.print(s);
			out.flush();

			String b = "";
			do{
				int read = in.read(buff, 0, 1024);
				String q = new String(buff, 0, read);
				b += q;
			}while(b.charAt(b.length()-1) != (char)0x17);
			System.out.println("{"+b+"}");
		}

		socket.close();
//*/
	}
}
