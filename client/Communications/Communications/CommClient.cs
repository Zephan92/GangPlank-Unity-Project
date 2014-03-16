using System;
using System.Text;
using System.Net.Sockets;

namespace Gangplank.Communications
{
    public class CommClient
    {
        private Socket socket = null;

        public CommClient() {
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.ReceiveTimeout = 5000;
        }

        public bool connect(string server, int port){
            try {
                socket.Connect(server, port);
                return true;
            }
            catch (SocketException) {
                return false;
            }
        }

        public bool connectMatchServer() {
            return connect(CommConstants.matchServer, CommConstants.matchPort);
        }

        public void close() {
            socket.Close();
        }

        public string sendAndRead(string msg) {
            send(msg);
            StringBuilder sb = new StringBuilder();
            byte[] rec = new byte[1024];
            int read;
            do {
                read = socket.Receive(rec);
                sb.Append(Encoding.ASCII.GetString(rec, 0, read));
            } while (rec[read - 1] != CommConstants.endTransmission);

            return sb.ToString();
        }
        public void send(string msg) {
            socket.Send(Encoding.ASCII.GetBytes(msg));
        }
    }
}