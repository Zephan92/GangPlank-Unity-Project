using UnityEngine;
using System.Collections;

/*
GangPlank StashCount - Updated: 3/03/14

This class is used to keep track of the stash values.
This class also displays the stash values on the interface.
*/

public class StashCount : MonoBehaviour {
	//All Variables
	public static float stash = 3f;//Player Start Stash Value
	public static float captainStashMultiplier;//Captain Start Multiplier
	public static float[] stashArray = new float[7];//All stash values 0 = captain: 1-6
	public static bool mutiny;
	private bool showLoseGame;//bool for the lose game interface


	//Calls only when the script turns on for the first time
	void Start()
	{
		captainStashMultiplier = 3f;
		showLoseGame = false;
		stashArray[0] = 1;//Assign Captain Stash
		mutiny = false;
		for(int i = 1; i < stashArray.Length;i++)//Assign stashes to players
		{stashArray[i] = stash;}
	}


	//Calls every update
	void Update()
	{
		if(stashArray[0] <= 0 && !mutiny)//This update is checking to see if the captains stash is used up. If so the game ends
		{
			PlayerTurnMenu.showOptions = false;//closes the interface
			PlayerTurnMenu.showCards = false;//closes the card displays
			PlayerTurnMenu.showChatWindow = false;//closes chat box
			showLoseGame = true;//opens the lose game display
			PlayerTurnMenu.mutinyCard1 = 0; PlayerTurnMenu.mutinyCard2 = 0; PlayerTurnMenu.mutinyCard3 = 0;
			stashArray[0] = 1;
		}
	}

	//Calls every update
	void OnGUI()
	{


		//If The interface is on, show all the stashes.
		if(PlayerTurnMenu.showOptions == true)
	  	{
			if(PlayerTurnMenu.playerTurn == 0)
			{
				stashArray[0] = captainStashMultiplier * StartMenu.numberOfPlayers;
				if(PlayerTurnMenu.round == 0)
				{
					for(int i = 1; i < stashArray.Length;i++)//Assign stashes to players
					{stashArray[i] = stash;}
				}
			}
			//Captain Stash
			GUI.Box(new Rect(Screen.width*25f/32f,
			                 Screen.height*2f/32f,
			                 Screen.width*6f/32f,
			                 Screen.height*2f/32f), "Captain's Gold Stash: " + stashArray[0]);

			//Player 1 Stash
			GUI.Box(new Rect(Screen.width*25f/32f,
			                 Screen.height*5f/32f,
			                 Screen.width*6f/32f,
			                 Screen.height*2f/32f), "Player One's Gold Stash: " + stashArray[1]);

			if(StartMenu.numberOfPlayers > 1)
			{
				//Player 2 Stash
				GUI.Box(new Rect(Screen.width*25f/32f,
				                 Screen.height*8f/32f,
				                 Screen.width*6f/32f,
				                 Screen.height*2f/32f), "Player Two's Gold Stash: " + stashArray[2]);
			}
			if(StartMenu.numberOfPlayers > 2)
			{
				//Player 3 Stash
				GUI.Box(new Rect(Screen.width*25f/32f,
				                 Screen.height*11f/32f,
				                 Screen.width*6f/32f,
				                 Screen.height*2f/32f), "Player Three's Gold Stash: " + stashArray[3]);
			}
			if(StartMenu.numberOfPlayers > 3)
			{
				//Player 4 Stash
				GUI.Box(new Rect(Screen.width*25f/32f,
				                 Screen.height*14f/32f,
				                 Screen.width*6f/32f,
				                 Screen.height*2f/32f), "Player Four's Gold Stash: " + stashArray[4]);
			}
			if(StartMenu.numberOfPlayers > 4)
			{
				//Player 5 Stash
				GUI.Box(new Rect(Screen.width*25f/32f,
				                 Screen.height*17f/32f,
				                 Screen.width*6f/32f,
				                 Screen.height*2f/32f), "Player Five's Gold Stash: " + stashArray[5]);
			}
			if(StartMenu.numberOfPlayers > 5)
			{
				//Player 6 Stash
				GUI.Box(new Rect(Screen.width*25f/32f,
				                 Screen.height*20f/32f,
				                 Screen.width*6f/32f,
				                 Screen.height*2f/32f), "Player Six's Gold Stash: " + stashArray[6]);
			}





		}

		//Debug.Log ("ShowLoseGame Menu = " + showLoseGame);
		if(showLoseGame == true)
		{
			if(PlayerTurnMenu.round > 2)
			{
				if(GUI.Button(new Rect(Screen.width*10f/32f,
				                       Screen.height*12f/32f,
				                       Screen.width*12f/32f,
				                       Screen.height*8f/32f), "The captain has ran out of his gold stash!" +
				                      						"\nThat was the last round, start a new game?"))
				{
					showLoseGame = false;
					Application.LoadLevel("Intermission");//Start the level over
					StartMenu.inputServerString = "";
					StartMenu.inputPasswordString = "";
					StartMenu.showStartMenu = true;//show main menu box
					StartMenu.showMakeGame = true;//show host/join menu
					StartMenu.showHostMenu = false;//Don't show host menu
					StartMenu.showJoinMenu = false;//Don't show join menu
				//	Debug.Log ("ShowLoseGame Menu = " + showLoseGame);
				}
			}
			else 
			{
				if(GUI.Button(new Rect(Screen.width*10f/32f,
				                       Screen.height*12f/32f,
				                       Screen.width*12f/32f,
				                       Screen.height*8f/32f), "The captain has ran out of his gold stash!" +
				                   							"\n          Start next round?"))
				{
					Application.LoadLevel("Intermission");//Start the level over
					showLoseGame = false;//closes the lose interface
					stashArray[0] = captainStashMultiplier * StartMenu.numberOfPlayers;
					for(int i = 1; i < stashArray.Length;i++)//Adds more stash at beginning of round
					{stashArray[i] += stash;}
					Application.LoadLevel("InGame");//Start the level over
					PlayerTurnMenu.showOptions = true;//opens the game interface
					PlayerTurnMenu.playerTurn = 0;
					//Debug.Log ("Restart Round");
				}
			}
		}
	}
}
