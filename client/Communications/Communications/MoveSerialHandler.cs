using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using SerialComm;

namespace Gangplank.Communications {
	internal class MoveSerialHandler {
		private delegate void SerialDelegate(string cmd, string msg);
		private Dictionary<String, SerialDelegate> handlers;
		private Dictionary<String, Card> cards;
		private serialComm serial;
		private CommManager comm;
		private string serialName{
			get{
				return comm.name+".serialPort";
			}
		}
		public MoveSerialHandler(serialComm serial, CommManager comm){
			this.serial = serial;
			this.comm = comm;
			handlers = new Dictionary<string,SerialDelegate>(){
				{"requestRFID", requestRFID}
				,{"requestPlayerUpdate", requestPlayerUpdate}
				,{"requestPlayerChoice", requestPlayerChoice}
				,{"requestPlayerMove", requestPlayerMove}
				,{"testSerial", testSerial}
				,{"testSerialTimeOut", testSerialTimeOut}
				,{"testSerialWrongData", testSerialWrongData}
			};
			cards = new Dictionary<string,Card>();
		}

		public void loadCardData(string filename){
			foreach(string line in File.ReadAllLines(filename)){
				string[] split = line.Split(',').Select(s => s.Trim()).ToArray();
				if(!cards.ContainsKey(split[0])){
					cards.Add(split[0], new Card(split[2], int.Parse(split[1])));
				}
			}
		}

		public void handleMove(string msg){
			string[] split = msg.Split(CommConstants.unitSplitArr, 2);
			if(handlers.ContainsKey(split[0])){
				try{
					Console.WriteLine("running "+split[0]);
					handlers[split[0]](split[0],split.Length>1?split[1]:null);
				}
				catch(Exception exc){
					Console.WriteLine(exc);
					comm.sendChat(exc.GetType()+" from operation "+split[0], serialName);
				}
			}
			else{
				comm.sendChat("not a serial command:"+msg, serialName);
			}
		}

		private void requestRFID(string cmd, string msg){
			string ans = serial.requestRFID(Byte.Parse(msg));
			comm.sendChat(cmd+"("+msg+"):"+"RFID "+ans+" "+(cards.ContainsKey(ans)?cards[ans].ToString():"no such card found"), serialName);
		}

		private void requestPlayerUpdate(string cmd, string msg){
			byte[] args = msg.Split(CommConstants.unitSplit).Select(Byte.Parse).ToArray();
			comm.sendChat(cmd+"("+args[0]+","+args[1]+"):"+
				serial.requestPlayerUpdate(args[0], args[1])
			, serialName);
		}

		private void requestPlayerChoice(string cmd, string msg){
			comm.sendChat(cmd+"("+msg+"):"+
				serial.requestPlayerChoice(Byte.Parse(cmd))
			, serialName);
		}

		private void requestPlayerMove(string cmd, string msg){
			byte[] args = msg.Split(CommConstants.unitSplit).Select(Byte.Parse).ToArray();
			comm.sendChat(cmd+"("+args[0]+","+args[1]+"):"+
				serial.requestPlayerMove(args[0], args[1])
			, serialName);
		}

		private void testSerial(string cmd, string msg){
			comm.sendChat(cmd+":"+
				serial.testSerial()
			, serialName);
		}

		private void testSerialTimeOut(string cmd, string msg){
			comm.sendChat(cmd+":"+
				serial.testSerialTimeOut()
			, serialName);
		}

		private void testSerialWrongData(string cmd, string msg){
			comm.sendChat(cmd+":"+
				serial.testSerialWrongData()
			, serialName);
		}
	}

	public class Card{

		public string type{get; private set;}
		public int id{get; private set;}

		public Card(string type, int id){
			this.type = type;
			this.id = id;
		}
		public override string ToString() {
			return "Card: Id "+id+" Type "+type;
		}
	}
}
