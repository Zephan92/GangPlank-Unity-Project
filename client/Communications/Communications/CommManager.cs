using System;
using System.Linq;
using System.Collections.Generic;

using SerialComm;

namespace Gangplank.Communications {
    public class CommManager {
		public static readonly CommResponse SUCCESS = new CommResponse(true, "ok", "success");

		public CommResponse lastFailure{get;private set;}
		internal String name{get;set;}

		CommClient client;
		serialComm serial;
		public CommManager() {
			client = new CommClient();
		}

		public CommResponse connectSerialPort(){
			serial = new serialComm();
			if(serial.openPortDialog()){
				serial = null;
				return new CommResponse(false, "err", "failed to open serial port");
			}
			MoveSerialHandler moveToSerial = new MoveSerialHandler(serial, this);
			addMoveListener(moveToSerial.handleMove);
			moveToSerial.loadCardData("RFID_records.txt");
			return SUCCESS;
		}

		public CommResponse closeSerialPort(){
			if(serial!= null && serial.closeSerial()){
				return new CommResponse(false, "err", "could not close serial port");
			}
			return SUCCESS;
		}

		public CommResponse connectToMatchServer(){
			try{
				client.connect(CommConstants.matchServer, CommConstants.matchPort);
			}catch(Exception exc){
				return new CommResponse(false, "err", exc.Message);
			}

			return SUCCESS;
		}

		private CommResponse sendAndRead(string msg){
			try{
				string res = client.sendAndRead(msg);
				string[] split = res.Split(CommConstants.unitSplitArr, 2);
				return new CommResponse(split[0].Equals("ok"), split[0], res);
			}catch(Exception exc){
				return new CommResponse(false, "err", exc.Message);
			}
		}

		private CommResponse send(string msg){
			try{
				client.send(msg);
				return SUCCESS;
			}catch(Exception exc){
				return new CommResponse(false, "err", exc.Message);
			}
		}

		public void addChatListener(UpdateDelegate callback){
			client.addHook("chat", callback);
		}

		public void addMoveListener(UpdateDelegate callback){
			client.addHook("move", callback);
		}

		public void addNewUserListener(UpdateDelegate callback){
			client.addHook("joined", callback);
		}

		public List<String> getHosts(int num = -1){
			string amt = num==-1?"all":num.ToString();
			CommResponse res = sendAndRead(composeMessage("list_hosts",amt));
			if(res.success) return res.message.Split(CommConstants.unitSplit).Skip(1).ToList();
			else lastFailure = res;
			return null;
		}

		public List<String> getGroupMembers(){
			CommResponse res = sendAndRead(composeMessage("list_group"));
			if(res.success) return res.message.Split(CommConstants.unitSplit).ToList();
			else lastFailure = res;
			return null;
		}

		public CommResponse hostGroup(string yourName){
			name = yourName;
			return sendAndRead(composeMessage("host", yourName));
		}

		public CommResponse joinGroup(string hostName, string yourName){
			name = yourName;
			return sendAndRead(composeMessage("join", hostName, yourName));
		}

		public CommResponse sendChat(string msg){
			if(msg.Length > 0 && msg[0] == CommConstants.unitSplit){
				return sendMove(msg.Substring(1)); //hack to demonstrate serial commands over network
			}
			return sendChat(msg, name);
		}

		internal CommResponse sendChat(string msg, string username){
			return send(composeMessage("group","chat", DateTime.Now.ToString("HH:mm:ss")+" "+username+": "+msg));
		}

		public CommResponse sendMove(string msg){
			return send(composeMessage("group", "move", msg));
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