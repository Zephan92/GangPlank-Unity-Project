using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace Gangplank.Communications {
    public class CommServer {
		private HashSet<UpdateDelegate> userListeners;
		private Thread listeningThread;

        public CommServer() {
			userListeners = new HashSet<UpdateDelegate>();
			listeningThread = new Thread(startAccepting);
		}

		public void startListening() {
			if (!listeningThread.IsAlive) {
				listeningThread.Start();
			}
		}

		public void stopListening() {
			listeningThread.Abort();
			listeningThread.Join();
		}

		private ManualResetEvent acceptEvent;
		private void startAccepting() {
			IPEndPoint localEndPoint = new IPEndPoint(0, 5555);
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			acceptEvent = new ManualResetEvent(false);
			try {
				listener.Bind(localEndPoint);
				listener.Listen(100);

				while (true) {
					acceptEvent.Reset();

					Console.WriteLine("Waiting for a connection...");
					listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

					acceptEvent.WaitOne();
				}

			}
			catch (ThreadAbortException e) { }
			catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
			//*/
		}

		private void AcceptCallback(IAsyncResult ar) {
			acceptEvent.Set();

			Socket listener = (Socket)ar.AsyncState;
			Socket handler = listener.EndAccept(ar);

			lock (userListeners) {
				foreach (UpdateDelegate update in userListeners) {
					update("connected:" + handler.RemoteEndPoint.ToString());
				}
			}
			//userListeners.(update => update());
			//handler.

			// Create the state object.
			//StateObject state = new StateObject();
			//state.workSocket = handler;
			/*
			handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
				new AsyncCallback(ReadCallback), state);
			 */
		}

		public void addUserListener(UpdateDelegate l) {
			lock (userListeners) userListeners.Add(l);
		}

		public void removeUserListener(UpdateDelegate l) {
			lock (userListeners) userListeners.Remove(l);
		}
    }
}
