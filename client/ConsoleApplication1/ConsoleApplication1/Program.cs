using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gangplank.Communications;

namespace ConsoleApplication1 {
    class Program {
        static void Main(string[] args) {

            CommManager comm = new CommManager();
				comm.addChatListener(str => Console.WriteLine(str));
				comm.addNewUserListener(str => Console.WriteLine(str+" has joined the group"));
				AppDomain.CurrentDomain.ProcessExit += new EventHandler((obj, evt)=> comm.closeSerialPort());

				CommResponse res;
				
				if(getFromUser("connect to serial port? (y/n)", "y", "n", "yes", "no")[0] == 'y'){
					res = comm.connectSerialPort();
					if(!res.success){
						exit(res.message);
					}
				}

				res = comm.connectToMatchServer();
				if(res.success){
					Console.WriteLine("Successfully connected to server");
				}
				else{
					exit(res.message);
				}

				while(true){
					if(getFromUser("host a new group or join an existing one? (enter host or join): ", "host", "join").Equals("host")){
						res = comm.hostGroup(getFromUser("enter your username: "));
						if(!res.success){
							Console.WriteLine(res.message);
							continue;
						}
					}
					else{
						string q = getFromUser("do you know the existing host's name? (enter their username or no): ");
						if(q.Equals("no")){
							List<String> hosts = comm.getHosts();
							if(hosts == null && comm.lastFailure.type.Equals("err")){
								exit(comm.lastFailure.message);
							}
							if(hosts == null || hosts.Count() == 0){
								Console.WriteLine("There aren't any hosts online");
								continue;
							}
							else{
								Console.WriteLine("Here is a list of hosts currently online:");
								hosts.ForEach(x => Console.WriteLine(x));
								continue;
							}
						}
						else{
							res = comm.joinGroup(q, getFromUser("enter your username: "));
							if(!res.success){
								Console.WriteLine(res.message);
								continue;
							}
						}

					}
					Console.WriteLine("Success, you can now chat with other members of your group.\nExit by pressing Ctrl+C");
					while(true){
						res = comm.sendChat(Console.ReadLine());
						if(!res.success) exit(res.message);
					}
				}
        }

		  static string getFromUser(string msg, params string[] options){
				string res;
				do{
					Console.Write(msg);
					res = Console.ReadLine();
				}while(!options.Any(x => x.Equals(res)));
				return res;
		  }
		  static string getFromUser(string msg){
				Console.Write(msg);
				return Console.ReadLine();
		  }

		  static void exit(string msg){
				Console.WriteLine(msg);
				Console.WriteLine("Press any key to exit");
            Console.ReadKey();
				Environment.Exit(0);
		  }
    }
}