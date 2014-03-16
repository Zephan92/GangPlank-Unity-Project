using System;
using System.Text;
using System.Threading;
using System.Linq;

namespace Gangplank.Communications {
    public class CommManager {

		static readonly Response connectionFail = new Response(false, "connection failed");
		static readonly Response alreadyHosting = new Response(false, "already hosting");

        private CommClient clientToCentral;
        private CommServer hostServer = null;
        private string hostName;

        public CommManager() { }

        public Response startHosting(string hostName, UpdateDelegate userUpdate) {
            if (hostServer != null) return alreadyHosting;
            this.hostName = hostName;
			clientToCentral = new CommClient();

            if (!clientToCentral.connectMatchServer()) return connectionFail;

            string res = clientToCentral.sendAndRead(composeMessage("add", hostName));
            bool success = messageTypeIs(res, "ok");
            if (success) {
                hostServer = new CommServer();
				hostServer.addUserListener(userUpdate);
				hostServer.startListening();
            }
            return new Response(success, res);
        }

        public static string messageType(string msg) {
            return msg.Substring(0, msg.IndexOf(CommConstants.unitSplit));
        }
        public static bool messageTypeIs(string msg, string type) {
            return messageType(msg).Equals(type);
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
    }
}
