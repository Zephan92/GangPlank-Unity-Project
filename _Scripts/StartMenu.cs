using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;

/*
GangPlank StartMenu - Updated: 2/25/14

This class is used to display the main menu interface.
It also has the variables for connecting to the server
*/

public class StartMenu : MonoBehaviour {
	public static bool showStartMenu;//If true, it shows the Main Menu Box
	public static bool showMakeGame;//If true, it shows the host/join buttons
	public static bool showHostMenu;//if true it shows the host interface
	public static bool showJoinMenu;//if true it shows the join interface
	public static string inputServerString;//Temporary server string
	public static string inputPasswordString;//Temporary password string
	private string saveServer;//server to connect too
	private string savePassword;//server password
	private float hSliderValue;//dynamic slider variable for player count
	public static float numberOfPlayers = 6f;//Player count

	public GUIStyle titleStyle;
	
	// Use this for initialization
	void Start () 
	{
		inputServerString = "";
		inputPasswordString = "";
		showStartMenu = true;//show main menu box
		showMakeGame = true;//show host/join menu
		showHostMenu = false;//Don't show host menu
		showJoinMenu = false;//Don't show join menu
		hSliderValue = 2.0f;
		Comm.init ();
	}
	
	
	//Calls every update
	void OnGUI()
	{
		
		if (showStartMenu == true)//if true, show main menu
		{
			//Main Menu Box
			GUI.Box(new Rect(Screen.width/8f,
			                 Screen.height*1/32,
			                 Screen.width - Screen.width/4f,Screen.height/4f), "GangPlank\n" +
			                 	"<size=30>Money, Mutiny and Mayhem</size>", titleStyle);
			

			//if true show main menu options
			if(showMakeGame == true)
			{
				//Show host option
				if(GUI.Button(new Rect(Screen.width/8f,Screen.height/3f,Screen.width - Screen.width/4f,Screen.height/4f), "Host a Game")) 
				{
					showHostMenu = true;//Turn on host menu
					showMakeGame = false;//Turn off host/join options
				}
				
				//show join option
				if(GUI.Button(new Rect(Screen.width/8f,Screen.height/3f + Screen.height/4f + Screen.height/16f,Screen.width - Screen.width/4f,Screen.height/4f), "Join a game")) 
				{
					showJoinMenu = true;//Turn on join menu
					showMakeGame = false;//Turn off host/join options
				}
			}
			
			
			//if true show host menu
			if(showHostMenu == true)
			{
				
				//Show server name box
				GUI.Box(new Rect(Screen.width*3f/32f,
				                 Screen.height*7f/32f,
				                 Screen.width*12f/32f,
				                 Screen.height*4f/32f), "Server Name");
				
				
				
				//server name variable text box
				inputServerString = GUI.TextArea(new Rect(Screen.width*17f/32f,
				                                          Screen.height*7f/32f,
				                                          Screen.width*12f/32f,
				                                          Screen.height*4f/32f), inputServerString);
				//show server password box
				GUI.Box(new Rect(Screen.width*3f/32f,
				                 Screen.height*12f/32f,
				                 Screen.width*12f/32f,
				                 Screen.height*4f/32f), "Server Password");
				
				
				//server name variable text box
				inputPasswordString = GUI.TextArea(new Rect(Screen.width*17f/32f,
				                                            Screen.height*12f/32f,
				                                            Screen.width*12f/32f,
				                                            Screen.height*4f/32f), inputPasswordString);
				
				
				//show player box
				GUI.Box(new Rect(Screen.width*3f/32f,
				                 Screen.height*17f/32f,
				                 Screen.width*12f/32f,
				                 Screen.height*4f/32f), "Players") ;
				
				
				//show player slider
				GUI.Label(new Rect(Screen.width*17f/32f,
				                   Screen.height*36f/64f,
				                   Screen.width*12f/32f,
				                   Screen.height*4f/32f),hSliderValue.ToString());
				
				
				//take the user input value for slider
				hSliderValue = GUI.HorizontalSlider(new Rect(Screen.width*18f/32f,
				                                             Screen.height*36f/64f,
				                                             Screen.width*11f/32f,
				                                             Screen.height*4f/32f), hSliderValue,2.0f, 6f);
				
				//round up the slider value
				hSliderValue = Mathf.Round(hSliderValue);
				
				
				//Create game button
				if(GUI.Button(new Rect(Screen.width*8/32f,
				                       Screen.height*26/32f,
				                       Screen.width*16/32f,
				                       Screen.height*4/32f), "Create a Game")) 
				{
					//if you don't have any input, don't create game
					if (inputServerString == ""|| inputPasswordString == "")
					{
						Debug.Log("You need to put in a server name and password");
					}
					else//else create a game
					{
						saveServer = inputServerString; //save the server
						savePassword = inputPasswordString;//save the password
						showHostMenu = false;//turn off the host menu
						showStartMenu = false;//turn off the start menu box
						numberOfPlayers = hSliderValue;//save the amount of players
						Application.LoadLevel("InGame");//Load the ingame level
						PlayerTurnMenu.playerTurn = 0;
						PlayerTurnMenu.showOptions = true;//turn on the player interface
						PlayerTurnMenu.showChatWindow = true;//turn on chat window
						PlayerTurnMenu.round = 0;
						Comm.hostGroup(saveServer);
					}
					
				}
				
				
				//Back button
				if(GUI.Button(new Rect(Screen.width*25f/32f,
				                       Screen.height*26f/32f,
				                       Screen.width*4f/32f,
				                       Screen.height*4f/32f), "Back"))
				{
					showHostMenu = false;//turn off this menu
					showStartMenu = true;//turn on main
					showMakeGame = true;//turn on main
				}
			}
			
			
			//if true turn on join menu
			if(showJoinMenu == true)
			{
				
				//server name box
				GUI.Box(new Rect(Screen.width*3f/32f,
				                 Screen.height*7f/32f,
				                 Screen.width*12f/32f,
				                 Screen.height*8f/32f), "Server Name");
				
				//server name input
				inputServerString = GUI.TextArea(new Rect(Screen.width*17f/32f,
				                                          Screen.height*7f/32f,
				                                          Screen.width*12f/32f,
				                                          Screen.height*8f/32f), inputServerString);
				
				
				//server password box
				GUI.Box(new Rect(Screen.width*3f/32f,
				                 Screen.height*16f/32f,
				                 Screen.width*12f/32f,
				                 Screen.height*8f/32f), "Server Password");
				
				
				//server password input
				inputPasswordString = GUI.TextArea(new Rect(Screen.width*17f/32f,
				                                            Screen.height*16f/32f,
				                                            Screen.width*12f/32f,
				                                            Screen.height*8f/32f), inputPasswordString);
				
				//try and join game
				if(GUI.Button(new Rect(Screen.width*8f/32f,
				                       Screen.height*26f/32f,
				                       Screen.width*16f/32f,
				                       Screen.height*4f/32f), "Join Game")) 
				{
					//if no data is inputed, don't go on
					if (inputServerString == ""|| inputPasswordString == "")
					{
						Debug.Log("You need to put in a server and password");
					}
					else //else join the game
					{
						saveServer = inputServerString;//save server
						savePassword = inputPasswordString;//save password
						showJoinMenu = false;//turn off this menu
						showStartMenu = false;//turn off main menu box
						Application.LoadLevel("InGame");//load the game
						PlayerTurnMenu.playerTurn = 0;
						PlayerTurnMenu.showOptions = true;//turn on player interface
						PlayerTurnMenu.showChatWindow = true;//turn on chat window
						PlayerTurnMenu.round = 0;
						Comm.joinGroup(saveServer, savePassword);
					}
					
				}
				
				
				//go back to main menu
				if(GUI.Button(new Rect(Screen.width*25f/32f,
				                       Screen.height*26f/32f,
				                       Screen.width*4f/32f,
				                       Screen.height*4f/32f), "Back"))
				{
					showJoinMenu = false;//turn off join menu
					showStartMenu = true;//turn on main menu box
					showMakeGame = true;//turn on main menu options
				}
			}
		}
	}
}

