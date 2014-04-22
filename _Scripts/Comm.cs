using UnityEngine;
using System.Collections;

using Gangplank.Communications;

public class Comm{
	//a script wrapper for the CommManager class, handling the network communications for the application

	private static CommManager comm;

	public static void init(){
		comm = new CommManager ();
		CommResponse res = comm.connectToMatchServer ();
		if (!res.success) {
			Debug.LogError(res.message);
			//TODO handle error with proper UI
		}
	}

	public static void addChatListener(UpdateDelegate callback){
	//	comm.addChatListener (callback);
	}
	public static void addNewUserListener (UpdateDelegate callback){
	//	comm.addNewUserListener (callback);
	}

	public static void addMoveListener(UpdateDelegate callback){
	//	comm.addMoveListener (callback);
	}

	public static void sendChat(string msg){
		CommResponse res = comm.sendChat (msg);
		if (!res.success) {
			Debug.LogError(res.message);
			//TODO handle error with proper UI
		}
	}

	public static void joinGroup(string hostname, string yourname){
		CommResponse res = comm.joinGroup (hostname, yourname);
		if (!res.success) {
			Debug.LogError(res.message);
			//TODO handle error with proper UI
		}
	}

	public static void hostGroup(string yourname){
		CommResponse res = comm.hostGroup (yourname);
		if (!res.success) {
			Debug.LogError(res.message);
			//TODO handle error with proper UI
		}
	}
}
