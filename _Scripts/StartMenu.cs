using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {
	private bool showStartMenu;
	private bool showMakeGame;
	private bool showHostMenu;
	private bool showJoinMenu;
	private string inputServerString;
	private string inputPasswordString;
	private string saveServer;
	private string savePassword;
	private float hSliderValue;


	void Start () 
	{
		inputServerString = "";
		inputPasswordString = "";
		showStartMenu = true;
		showMakeGame = true;
		showHostMenu = false;
		showJoinMenu = false;
	}
	

	void Update () 
	{
		//GetComponent(NameofScript).enabled = false;
	}

	void OnGUI()
	{

		if (showStartMenu == true)
		{
			GUI.Box(new Rect(Screen.width/64,Screen.height/32,Screen.width - Screen.width/32,Screen.height - Screen.height/16), "GangPlank");

			if(showMakeGame == true)
			{
				if(GUI.Button(new Rect(Screen.width/8,Screen.height/3,Screen.width - Screen.width/4,Screen.height/4), "Host a Game")) 
				{
					showHostMenu = true;
					showMakeGame = false;
				}
				
				if(GUI.Button(new Rect(Screen.width/8,Screen.height/3 + Screen.height/4 + Screen.height/16,Screen.width - Screen.width/4,Screen.height/4), "Join a game")) 
				{
					showJoinMenu = true;
					showMakeGame = false;
				}
			}

			if(showHostMenu == true)
			{

				GUI.Box(new Rect(Screen.width*3/32,
				                 Screen.height*3/32,
				                 Screen.width*12/32,
				                 Screen.height*4/32), "Server Name");

				inputServerString = GUI.TextArea(new Rect(Screen.width*17/32,
				                                          Screen.height*3/32,
				                                          Screen.width*12/32,
				                                          Screen.height*4/32), inputServerString);
				
				GUI.Box(new Rect(Screen.width*3/32,
				                 Screen.height*8/32,
				                 Screen.width*12/32,
				                 Screen.height*4/32), "Server Password");

				inputPasswordString = GUI.TextArea(new Rect(Screen.width*17/32,
				                                            Screen.height*8/32,
				                                            Screen.width*12/32,
				                                            Screen.height*4/32), inputPasswordString);
				GUI.Box(new Rect(Screen.width*3/32,
				                 Screen.height*13/32,
				                 Screen.width*12/32,
				                 Screen.height*4/32), "Players") ;

				hSliderValue = GUI.HorizontalSlider(new Rect(Screen.width*17/32,
				                                             Screen.height*29/64,
				                                             Screen.width*12/32,
				                                             Screen.height*4/32), hSliderValue,0.0f, 6.0f);
				hSliderValue = Mathf.Round(hSliderValue);

				if(GUI.Button(new Rect(Screen.width*8/32,
				                       Screen.height*26/32,
				                       Screen.width*16/32,
				                       Screen.height*4/32), "Create a Game")) 
				{
					if (inputServerString == ""|| inputPasswordString == "")
					{
						Debug.Log("You need to put in a server and password");
					}
					else
					{
						saveServer = inputServerString;
						savePassword = inputPasswordString;
						showHostMenu = false;
						showStartMenu = false;
						PlayerTurnMenu.showOptions = true;
						ChangeTurn_Camera.inGame = true;
						Application.LoadLevel("InGame");
					}

				}
			}
			if(showJoinMenu == true)
			{
				GUI.Box(new Rect(Screen.width*3/32,
				                 Screen.height*7/32,
				                 Screen.width*12/32,
				                 Screen.height*8/32), "Server Name");
				
				inputServerString = GUI.TextArea(new Rect(Screen.width*17/32,
				                                          Screen.height*7/32,
				                                          Screen.width*12/32,
				                                          Screen.height*8/32), inputServerString);
				
				GUI.Box(new Rect(Screen.width*3/32,
				                 Screen.height*16/32,
				                 Screen.width*12/32,
				                 Screen.height*8/32), "Server Password");
				
				inputPasswordString = GUI.TextArea(new Rect(Screen.width*17/32,
				                                            Screen.height*16/32,
				                                            Screen.width*12/32,
				                                            Screen.height*8/32), inputPasswordString);

				if(GUI.Button(new Rect(Screen.width*8/32,
				                       Screen.height*26/32,
				                       Screen.width*16/32,
				                       Screen.height*4/32), "Join Game")) 
				{
					if (inputServerString == ""|| inputPasswordString == "")
					{
						Debug.Log("You need to put in a server and password");
					}
					else
					{
						saveServer = inputServerString;
						savePassword = inputPasswordString;
						showJoinMenu = false;
						showStartMenu = false;
						PlayerTurnMenu.showOptions = true;
						ChangeTurn_Camera.inGame = true;
						Application.LoadLevel("InGame");
					}

				}
			}
		}
	}
}
