using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

namespace Gangplank.Communications
{
	public class CommClient{
		private Socket socket;
		private BlockingQueue<String> queue;
		private const int BUFF_SIZE = 1024;
		private byte[] buffer = new byte[BUFF_SIZE];
		private StringBuilder builder;
		private Dictionary<String, List<UpdateDelegate>> hooks;

		public CommClient() {
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			queue = new BlockingQueue<string>(20);
			builder = new StringBuilder();
			hooks = new Dictionary<string,List<UpdateDelegate>>();
		}

		public void connect(string server, int port){
			IPAddress ipAddress = Dns.GetHostAddresses(server)[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			socket.Connect(remoteEP);

			socket.BeginReceive(buffer, 0, BUFF_SIZE, 0, new AsyncCallback(ReceiveCallback), null);
		}

		private void ReceiveCallback(IAsyncResult ar){
			try{
				int read = socket.EndReceive(ar);

				if(read > 0){
					builder.Append(Encoding.ASCII.GetString(buffer, 0, read));

					if(buffer[read-1] == CommConstants.endTransmission){
						processMessage(builder.ToString().TrimEnd(CommConstants.endTransmission));
						builder = new StringBuilder();
					}

					socket.BeginReceive(buffer, 0, BUFF_SIZE, 0, new AsyncCallback(ReceiveCallback), null);
				}
			}catch(Exception exc){
				Console.WriteLine(exc);
			}
		}

		private void processMessage(string msg){
			String[] split = msg.Split(CommConstants.unitSplitArr, 3);
			if(split.Length > 2 && split[0].Equals("hook")){
				callHook(split[1], split[2]);
				return;
			}

			queue.Enqueue(msg);
		}

		private void callHook(string name, string msg){
			if(hooks.ContainsKey(name)) hooks[name].ForEach(del => del(msg));
		}

		public void addHook(string name, UpdateDelegate callback){
			List<UpdateDelegate> list;
			if(hooks.ContainsKey(name)){
				list = hooks[name];
			}
			else{
				list = new List<UpdateDelegate>();
				hooks.Add(name, list);
			}
			list.Add(callback);
		}	
		public void removeHook(string name, UpdateDelegate callback){
			if(hooks.ContainsKey(name)){
				hooks[name].Remove(callback);
			}
		}

		public string blockForMessage(){
			return queue.Dequeue();
		}

		public void send(string msg){
			lock(socket){
				socket.Send(Encoding.ASCII.GetBytes(msg));
			}
		}

		public string sendAndRead(string msg){
			lock(socket){
				send(msg);
				return blockForMessage();
			}
		}
	}
}