using System;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Gangplank.Communications {
    public class CommManager {
		public static readonly Response SUCCESS = new Response(true, "ok", "success");

		public Response lastFailure{get;set;}
		private String name{get;set;}

		CommClient client;
        public CommManager() {
			client = new CommClient();
		}

		public Response connectToMatchServer(){
			try{
				client.connect(CommConstants.matchServer, CommConstants.matchPort);
			}catch(Exception exc){
				return new Response(false, "err", exc.Message);
			}

			return SUCCESS;
		}

		private Response sendAndRead(string msg){
			try{
				string res = client.sendAndRead(msg);
				string[] split = res.Split(CommConstants.unitSplitArr, 2);
				return new Response(split[0].Equals("ok"), split[0], res);
			}catch(Exception exc){
				return new Response(false, "err", exc.Message);
			}
		}

		private Response send(string msg){
			try{
				client.send(msg);
				return SUCCESS;
			}catch(Exception exc){
				return new Response(false, "err", exc.Message);
			}
		}

		public void addChatListener(UpdateDelegate callback){
			client.addHook("chat", callback);
		}

		public void addNewUserListener(UpdateDelegate callback){
			client.addHook("joined", callback);
		}

		public List<String> getHosts(int num = -1){
			string amt = num==-1?"all":num.ToString();
			Response res = sendAndRead(composeMessage("list_hosts",amt));
			if(res.success) return res.message.Split(CommConstants.unitSplit).Skip(1).ToList();
			else lastFailure = res;
			return null;
		}

		public List<String> getGroupMembers(){
			Response res = sendAndRead(composeMessage("list_group"));
			if(res.success) return res.message.Split(CommConstants.unitSplit).ToList();
			else lastFailure = res;
			return null;
		}

		public Response hostGroup(string yourName){
			name = yourName;
			return sendAndRead(composeMessage("host", yourName));
		}

		public Response joinGroup(string hostName, string yourName){
			name = yourName;
			return sendAndRead(composeMessage("join", hostName, yourName));
		}

		public Response sendChat(string msg){
			return send(composeMessage("group","chat", DateTime.Now+" "+name+":"+msg));
		}

        private static string composeUnits(params string[] units) {
            return String.Join(CommConstants.unitSplit.ToString(), units);
        }
        private static string composeMessageMulti(params string[] records) {
            return String.Join(CommConstants.recordSplit.ToString(), records) + CommConstants.endTransmission;
        }
        private static string composeMessage(params string[] units) {
            return composeMessageMulti(composeUnits(units));
        }
		private static string composeMessage(string cmd, string[] args) {
			return composeMessageMulti(composeUnits(cmd, composeUnits(args)));
		}
    }
}