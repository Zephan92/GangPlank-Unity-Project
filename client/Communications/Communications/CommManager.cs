using System;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Gangplank.Communications {
    public class CommManager {
		public static readonly Response CONNECTION_FAILED = new Response(false, "failed to connect to server"),
		                                SUCCESS = new Response(true, "success");

		CommClient client;
        public CommManager() {
			client = new CommClient();
		}

		public Response connectToMatchServer(){
			try{
				client.connect(CommConstants.matchServer, CommConstants.matchPort);
			}catch(Exception exc){
				return new Response(false, exc.Message);
			}

			return SUCCESS;
		}

		private Response sendAndRead(string msg){
			try{
				string[] split = client.sendAndRead(msg).Split(CommConstants.unitSplitArr, 2);
				return new Response(split[0].Equals("ok"), split[split.Length>1?1:0]);
			}catch(Exception exc){
				return new Response(false, exc.Message);
			}
		}

		public void addChatListener(UpdateDelegate callback){
			client.addHook("chat", callback);
		}

		public void addNewUserListener(UpdateDelegate callback){
			client.addHook("joined", callback);
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