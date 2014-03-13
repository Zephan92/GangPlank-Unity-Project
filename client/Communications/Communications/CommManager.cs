using System;
using System.Text;
using System.Threading;

namespace Gangplank.Communications {
    public class CommManager {

		  static readonly Response connectionFail = new Response(false, "err;connection failed");
		  static readonly Response alreadyHosting = new Response(false, "err;already hosting");

        CommClient clientToCentral;
		  //Comm
        string hostName;
        UpdateListener newUserListener;
        string hostServer = null;

        public CommManager() { }

        public Response startHosting(string hostName, UpdateListener newUserListener) {
            if (hostServer != null) return alreadyHosting;
            this.hostName = hostName;
				clientToCentral = new CommClient();

            if (!clientToCentral.connectMatchServer()) return connectionFail;

				string res = clientToCentral.sendAndRead("add;" + hostName);
				bool success = messageTypeIs(res, "ok");
				if (success) {
					//hostServer
				}
				return new Response(success, res);
        }

        public static string messageType(string msg) {
            return msg.Substring(0, msg.IndexOf(CommConstants.unitSplit));
        }
        public static bool messageTypeIs(string msg, string type) {
            return messageType(msg).Equals(type);
        }
    }
}
