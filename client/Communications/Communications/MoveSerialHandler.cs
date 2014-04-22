using System;
using System.Collections.Generic;
using System.Linq;

using SerialComm;

namespace Gangplank.Communications {
	internal class MoveSerialHandler {
		private Dictionary<String, UpdateDelegate> handlers;
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
			handlers = new Dictionary<string,UpdateDelegate>(){
				{"readRFID", readRFID}
				,{"updatePlayer", updatePlayer}
				,{"testSerial", testSerial}
				,{"testSerialTimeOut", testSerialTimeOut}
				,{"testSerialWrongData", testSerialWrongData}
			};
			//serialName = comm.name+".serialPort";
		}

		public void handleMove(string msg){
			string[] split = msg.Split(CommConstants.unitSplitArr, 2);
			if(handlers.ContainsKey(split[0])){
				try{
					handlers[split[0]](split.Length>1?split[1]:null);
				}
				catch(Exception exc){
					Console.WriteLine(exc);
					comm.sendChat(exc.GetType()+" from operation "+split[0], serialName);
				}
			}
			else{
				Console.WriteLine("not a serial command:"+msg);
			}
		}
		private void readRFID(string msg){
			Console.WriteLine("trying to read RFID...");
			comm.sendChat(serial.readRFID(), serialName);
		}
		private void updatePlayer(string msg){
			byte[] args = msg.Split(CommConstants.unitSplit).Select(Byte.Parse).ToArray();
			Console.WriteLine("updating player {0} to pos {1}", args[0], args[1]);
			comm.sendChat("updatePlayer("+args[0]+","+args[1]+":"+
				serial.updatePlayer(args[0], args[1])
			, serialName);
		}
		private void testSerial(string msg){
			Console.WriteLine("testing serial");
			comm.sendChat("testSerial:"+
				serial.testSerial()
			, serialName);
		}
		private void testSerialTimeOut(string msg){
			Console.WriteLine("testing serial timeout");
			comm.sendChat("testSerialTimeOut:"+
				serial.testSerialTimeOut()
			, serialName);
		}
		private void testSerialWrongData(string msg){
			Console.WriteLine("testing serial wrong data");
			comm.sendChat("testSerialWrongData:"+
				serial.testSerialWrongData()
			, serialName);
		}
	}
}
