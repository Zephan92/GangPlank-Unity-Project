﻿using UnityEngine;
using System.Collections;


//Need to change around when to pick up your cards
//Need to always fill the first 3-5 slots
//Show digital players, their hand when they have no turn.

public class PlayerTurnMenu : MonoBehaviour {
	public static bool showOptions;
	public static bool showCards;
	public static bool showCaptainCard;
	public static bool showChoosePlayer;
	public static bool showChatWindow;
	public static bool showMutinyCards;
	public static bool[] showPathChoice = new bool[17];//0 = turn on display, 1-13 = horizontal, 14-16 = plank
	public static int playerTurn;
	public static int playerLoseTurn;
	public static int targetPlayer;
	public static int targetMethod;
	public static int targetAmount;
	public static int round;
	public static int numOfCapCards;
	public static bool useMethod;
	private int mutinyCard1 = 0;
	private int mutinyCard2 = 0;
	private int mutinyCard3 = 0;
	public int cardRangeMin = 1;
	public int cardRangeMax = 27;
	public Cards cards;
	private GameObject currentPlayer;
	private GameObject movePlayerObject;
	private PlayerStats ps2;
	private int[] cardsArray;
	private string[] cardDisplaysArray = new string[10];
	private int displayCaptainCard;
	public PlayerStats ps;
	private int randomCard;
	private bool stealCoins;
	private bool populateHand;
	private bool startOfTurn;


	public string chatText, fieldText;
	private Vector2 chatScroll = new Vector2(0, 0);

	private GameObject tempPlayer;
	private PlayerStats tempStats;

	public GUIStyle buttonstyle;

	//test var
	public Texture testTexture;

	//For generating tile selection for cards
	public GUIStyle generalTile;
	public GUIStyle offTile;
	public GUIStyle startTile;
	public GUIStyle weaponTile;
	public GUIStyle restraintTile;
	public GUIStyle hintTile;
	public GUIStyle plankTile;

	// Use this for initialization
	void Start () 
	{
		round = 0;
		playerTurn = -1;
		showPathChoice[0] = false;
		showOptions = false;
		showCards = false;
		showCaptainCard = false;
		showChatWindow = false;
		showChoosePlayer = false;
		stealCoins = false;
		useMethod = false;
		populateHand = false;
		numOfCapCards = 1;
		showMutinyCards = false;
		startOfTurn = false;


		//having trouble integrating...
		//Comm.init ();
		Comm.addChatListener (str => {chatText += str+"\n"; chatScroll = new Vector2(chatScroll.x, Mathf.Infinity);});
		Comm.addNewUserListener (str => {chatText += "Player "+str+" has joined the game\n"; chatScroll = new Vector2(chatScroll.x, Mathf.Infinity);});
		//Comm.joinGroup ("renar", "foobar");
	}

	/*void Update()
	{
		Debug.Log ("Player Stats is: " + ps.ToString());
	}*/

	private void changePlayer()
	{
		if(playerTurn == 0)
		{	
			round++;
			playerTurn = 1;
		}
		else if (playerTurn == StartMenu.numberOfPlayers)
		{
			playerTurn = 1;
		}
		else
		{
			playerTurn++;
		}
		startOfTurn = true;
		if (playerTurn == playerLoseTurn)
		{
			Debug.Log(ps.name.ToString() + " Lost There Turn!");
			playerLoseTurn = 0;
			changePlayer();
		}
		currentPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + playerTurn);
		ps = currentPlayer.GetComponent("PlayerStats") as PlayerStats;
		ps.addPlays(2);
		AddCardsToHand(0);
		(Camera.main.GetComponent("FollowPlayer")as FollowPlayer).UpdateCamera(playerTurn);
		//Debug.Log ("ps = " + ps.ToString());
	}


	private void displayCards()
	{
		for(int i = 0; i < cardDisplaysArray.Length; i++)
			cardDisplaysArray[i] = cards.getCardDescription(cardsArray[i]);
	}
	
	public void AddCardsToHand(int add)
	{
		float loop = playerTurn;
		if(populateHand == true)
		{
			loop = StartMenu.numberOfPlayers;
			populateHand = false;
		}
		
		for(int j = playerTurn; j <= loop; j++)
		{
			ps = (GameObject.Find("/Pirate_Ship/Players/Player_" + j)).GetComponent("PlayerStats") as PlayerStats;
			cardsArray = ps.getCards();
			ps.addCards(add);
			for(int i = 0; i < cardsArray.Length;i++)
			{
				//Debug.Log ("Cards to Add: " + ps.getCardsToAdd());
				if(cardsArray[i] == 0 && ps.getCardsToAdd() > 0)
				{
					randomCard = Random.Range(cardRangeMin,cardRangeMax+1);
					randomCard = (int) Mathf.Round(randomCard);
					cardsArray[i] = randomCard;
					ps.addCards(-1);
					//Debug.Log ("Adding Card to card slot " + (i+1));
				}
				if(cardsArray[i] == 12 && ps.getDrawRopes() == false)
				{
					cardsArray[i] = 1;
				}
				//Debug.Log("Card "+(i+1)+" is card #"+cardsArray[i]+" for player "+j);
			}
			ps.setCards(cardsArray);// update player hand
		}
		ps = (GameObject.Find("/Pirate_Ship/Players/Player_" + playerTurn)).GetComponent("PlayerStats") as PlayerStats;
		displayCards();
	}
	
	//probably needs to be handled in some way for drawing multiple Captain cards
	//Though players don't get to choose order...
	//so captain cards should still only come up one at a time?
	public void drawCaptainCard(){
		randomCard = Random.Range(1,25);
		randomCard = (int) Mathf.Round(randomCard);
		displayCaptainCard = randomCard;
		//displayCaptainCard = 6; //test specific cards
	}


	/*public void ResetHand()
	{
		ps.setCardsToAdd(1);
		cardsArray = ps.getCards();
		for(int i = 0; i < cardsArray.Length;i++)
		{
			cardsArray[i] = 0;
			if(cardsArray[i] == 0 && ps.getCardsToAdd() > 0)
			{
				randomCard = Random.Range(1,3);
				randomCard = (int) Mathf.Round(randomCard);
				cardsArray[i] = randomCard;
			}
			
		}
		ps.setCards(cardsArray);// update player hand
		displayCards();
	}*/

	public void targetPlayerMethod(int method, int target)
	{
		ps = (GameObject.Find("/Pirate_Ship/Players/Player_" + target)).GetComponent("PlayerStats") as PlayerStats;
		
		switch(method)
		{
		case 1:
			movePlayer(target,targetAmount);
			break;
			
		case 2:
			if(StashCount.stashArray[target] - targetAmount >= 0)
			{
				StashCount.stashArray[target] -= targetAmount;
				StashCount.stashArray[playerTurn] += targetAmount;
			}
			else if(StashCount.stashArray[target] - targetAmount < 0)
			{
				StashCount.stashArray[playerTurn] += StashCount.stashArray[target];
				StashCount.stashArray[target] = 0;
			}
			break;
			
		case 3:
			ps.addPlays(targetAmount);
			ps.setLostCards(ps.getLostCards() + 1);
			break;
			
		case 4:
			movePlayerObject= GameObject.Find("/Pirate_Ship/Players/Player_" + target) as GameObject;//Target Player
			ps2 = movePlayerObject.GetComponent("PlayerStats")as PlayerStats;//target player
			ps = currentPlayer.GetComponent("PlayerStats") as PlayerStats;//current player
			int current = ps2.getCurrentSpace();
			movePlayerHelper(ps.getCurrentSpace());//move target to player
			if(targetAmount > 0)
			{
				movePlayerObject= GameObject.Find("/Pirate_Ship/Players/Player_" + playerTurn) as GameObject;//current Player
				ps2 = movePlayerObject.GetComponent("PlayerStats")as PlayerStats;//current player
				movePlayerHelper(current);//move current player to target
			}
			break;
			
		case 5:
			playerLoseTurn = target;
			break;
			
		case 6:
			ps.setDrawRopes(false);
			break;
			
		case 7:
			movePlayerObject= GameObject.Find("/Pirate_Ship/Players/Player_" + target) as GameObject;//current Player
			ps2 = movePlayerObject.GetComponent("PlayerStats")as PlayerStats;//current player
			showPathChoice[4] = true;
			showPathChoice[10] = true;
			showPathChoice[13] = true;
			showPathChoice[0] = true;
			break;
			
		case 8:
			if (stealCoins == false)
			{
				stealCoins = true;
				useMethod = true;
				movePlayer(PlayerTurnMenu.playerTurn,1);
			}
			else
			{
				for(int i = 1; i <= StartMenu.numberOfPlayers; i++)
				{
					movePlayerObject= GameObject.Find("/Pirate_Ship/Players/Player_" + i) as GameObject;//Target Player
					ps2 = movePlayerObject.GetComponent("PlayerStats")as PlayerStats;//target player
					if(ps.getCurrentSpace() == ps2.getCurrentSpace())
					{
						targetPlayerMethod(2,i);
					}
				}
				stealCoins = false;
			}
			break;
			
		case 9:
			movePlayer(target,1);
			targetPlayerMethod(2,target);
			break;
			
		case 10:
			StashCount.stashArray[0] -= 5;
			int amount = 5;
			int player = playerTurn;
			
			if (player == StartMenu.numberOfPlayers)
			{
				player = 1;
			}
			else
			{
				player++;
			}
			for(int i = 0; i < StartMenu.numberOfPlayers; i++)
			{
				amount--;
				StashCount.stashArray[player] += 1;
				if (player == StartMenu.numberOfPlayers)
				{
					player = 1;
				}
				else
				{
					player++;
				}
			}
			StashCount.stashArray[playerTurn] += amount;
			break;
			
		default:
			Debug.Log("You need to implement that method");
			break;
		}
		ps = currentPlayer.GetComponent("PlayerStats") as PlayerStats;
	}


	public void movePlayer(int player,int spaces)
	{
		movePlayerObject= GameObject.Find("/Pirate_Ship/Players/Player_" + player) as GameObject;
		ps2 = movePlayerObject.GetComponent("PlayerStats")as PlayerStats;
		int start, end, endPlank, startPlank, current = ps2.getCurrentSpace();
		
		if (current > 13)
		{
			if((7 - (spaces - (current - 13)) < 1))
			{start = 1;}
			else
			{start = (7 - (spaces - (current - 13)));}
			
			if(7 + (spaces - (current - 13)) > 13)
			{end = 13;}
			else
			{end = (7 + (spaces - (current - 13)));}
			
			if (current - spaces <= 13)
			{
				for(int i = start; i <= end; i++)
				{showPathChoice[i] = true;}
			}
			
			if(current - spaces > 13)
			{
				startPlank = current - spaces;
			}
			else
			{
				startPlank = 14;
			}
			
			if (current + spaces < 16)
			{
				endPlank = current + spaces;
			}
			else
			{
				endPlank = 16;
			}
			
			for(int j = startPlank; j <= endPlank; j++)
			{showPathChoice[j] = true;}
		}
		else
		{
			if(current - spaces < 1)
			{start = 1;}
			else
			{start = current - spaces;}
			
			if(current + spaces > 13)
			{end = 13;}
			else
			{end = current + spaces;}
			
			for(int i = start; i <= end; i++)
			{showPathChoice[i] = true;}
			
			if(current <= 7 && current + spaces > 7)
			{
				endPlank = (current + (spaces - 7)) + 13;
				if (endPlank > 16)
				{endPlank = 16;}
				
				for(int k = 14; k <= endPlank; k++)
				{showPathChoice[k] = true;}
			}
			else if (current - spaces < 7 && current > 7)
			{
				endPlank = (7 - (current - spaces)) + 13;
				if (endPlank > 16)
				{endPlank = 16;}
				
				for(int k = 14; k <= endPlank; k++)
				{showPathChoice[k] = true;}
			}
		}
		showPathChoice[0] = true;
	}
	public void movePlayerHelper(int space)
	{
		int current = ps2.getCurrentSpace();
		ps2.setCurrentSpace(space);
		
		if (current > 13 && space > 13)
		{
			movePlayerObject.transform.position = movePlayerObject.transform.position + new Vector3(0,0,(current-space)*6);
		}
		else if(current > 13)
		{
			movePlayerObject.transform.position = movePlayerObject.transform.position + new Vector3((space-7)*6,0,(current-13)*6);
		}
		else if(space > 13)
		{
			movePlayerObject.transform.position = movePlayerObject.transform.position + new Vector3((7-current)*6,0,(13-space)*6);
		}
		else
		{
			movePlayerObject.transform.position = movePlayerObject.transform.position + new Vector3((space-current)*6,0,0);
		}
		(Camera.main.GetComponent("FollowPlayer")as FollowPlayer).UpdateCamera(playerTurn);
		for(int k = 1; k <= 16; k++)
		{showPathChoice[k] = false;}
		if(useMethod == true)
		{
			useMethod = false;
			targetPlayerMethod(8,playerTurn);
		}
	}
	
	//copying to avoid breaking normal movement
	//meant for moving that doesn't involved a choice
	//i.e. moving to a specific location or forced toward Overboard
	public void changePlayerLocation(int player, int space)
	{
		tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + player);
		tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
		
		int current = tempStats.getCurrentSpace();
		tempStats.setCurrentSpace(space);
		
		if (current > 13 && space > 13)
		{
			tempPlayer.transform.position = tempPlayer.transform.position + new Vector3(0,0,(current-space)*6);
		}
		else if(current > 13)
		{
			tempPlayer.transform.position = tempPlayer.transform.position + new Vector3((space-7)*6,0,(current-13)*6);
		}
		else if(space > 13)
		{
			tempPlayer.transform.position = tempPlayer.transform.position + new Vector3((7-current)*6,0,(13-space)*6);
		}
		else
		{
			tempPlayer.transform.position = tempPlayer.transform.position + new Vector3((space-current)*6,0,0);
		}
		(Camera.main.GetComponent("FollowPlayer")as FollowPlayer).UpdateCamera(playerTurn);
	}
	
	public void moveTowardOverboard(int player,int spaces)
	{
		tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + player);
		tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
		int startPlank = 7, current = tempStats.getCurrentSpace();
		int movement = spaces;
		bool plank = true; //skip the last if (#3) if moving is complete
		
		//1. look from start to startPlank
		if (current < startPlank) {
			//Debug.Log("entering #1");
			if(startPlank - current >= movement){ //player doesn't reach plank
				current = current + movement;
				plank = false;
			}
			else {  //gonna end up farther onto plank
				movement = movement - (startPlank - current);
				current = startPlank;
				//Debug.Log("Current: " + current + "; spaces : " + movement);
			}
		}
		//2. look from end to startPlank
		else if (current > startPlank && current <= 13) {	
			//Debug.Log("entering #2");
			if(current - startPlank >= movement){ //player doesn't reach plank
				current = current - movement;
				plank = false;
			}
			else {  //gonna end up farther onto plank
				movement = movement - (current - startPlank);
				current = startPlank;
			}
		}
		//3. look from startPlank to endPlank
		if((current == 7 || current > 13) && plank){
			//Debug.Log("entering #3");
			if(current == 7 && movement == 1)
				current = 14;
			else if(current == 7 && (movement > 1 || movement <= 3)){
				//Debug.Log("entering else if");
				current = 14 + (movement - 1);
			}
			else
				current = 16;
		}
		
		//finally, actually move the player
		changePlayerLocation (player, current); //at this point, current is actually the new location
		
	}


	void OnGUI()
	{
		if (showOptions == true)
		{
			//Debug.Log ("In Show Options");
			if (ps == null && playerTurn == 0 && GameObject.Find("/Pirate_Ship/Players/Player_1") != null)
			{
				//Debug.Log ("Change player 0 to player 1");
				cards = gameObject.GetComponent("Cards") as Cards;
				populateHand = true;
				changePlayer();
				AddCardsToHand(0);
				
			}
			else if(ps != null && playerTurn > 0)
			{
				//Debug.Log("current player is " + ps.ToString());
				//Debug.Log ("In player interface");
				GUI.Box(new Rect(Screen.width*1/32,
				                 Screen.height*2/32,
				                 Screen.width*6/32,
				                 Screen.height*4/32), "\nPlayer " + playerTurn);
				
				
				GUI.Box(new Rect(Screen.width*8/32,
				                 Screen.height*2/32,
				                 Screen.width*3/32,
				                 Screen.height*4/32), "\n" + "Cards: " + ps.getPlays());
				
				
				GUI.Box(new Rect(Screen.width*12/32,
				                 Screen.height*2/32,
				                 Screen.width*6/32,
				                 Screen.height*4/32), "\nRound " + round);
				
				if (ps.getPlays() > 0f)
				{
					GUI.Box(new Rect(Screen.width*1/32,
					                 Screen.height*7/32,
					                 Screen.width*6/32,
					                 Screen.height*4/32), "\n End Turn");
				}
				else
				{
					if(GUI.Button(new Rect(Screen.width*1/32,
					                       Screen.height*7/32,
					                       Screen.width*6/32,
					                       Screen.height*4/32), "\n End Turn")) 
					{
						ps.addCards(2 - ps.getLostCards());
						ps.setLostCards(0);
						if(numOfCapCards > 0){
							drawCaptainCard();
							showOptions = false;
							showCaptainCard = true;
						}
						else {
							numOfCapCards = 1; //reset to regular value
							changePlayer();
						}
						//note, if a captain card is played, changePlayer() will happen
						//after that captain card is clicked
						//here, it changes player if no captain cards are to be played
						//that turn
					}
				}
				
				
				if (ps.getPlays() == 0f )
				{
					GUI.Box(new Rect(Screen.width*1f/32f,
					                 Screen.height*12f/32f,
					                 Screen.width*6f/32f,
					                 Screen.height*4f/32f), "\nPlay A Card");
				}
				else
				{
					if(GUI.Button(new Rect(Screen.width*1/32,
					                       Screen.height*12/32,
					                       Screen.width*6/32,
					                       Screen.height*4/32), "Play A Card")) 
					{
						//Debug.Log("Inside Play A card button");
						///Debug.Log("Number of cards to play: " + ps.getCardsToAdd());
						//Debug.Log(optionString);
						if (ps.getPlays() > 0)
						{
							AddCardsToHand(0);
							showOptions = false;
							showCards = true;
						}
					}
				}
				
				if (ps.getPlays() > 0f && (ps.getCurrentSpace() == 4 || ps.getCurrentSpace() == 10 || ps.getCurrentSpace() == 13))//need to check if they are on the correct space
				{
					if(GUI.Button(new Rect(Screen.width*1/32,
					                       Screen.height*17/32,
					                       Screen.width*6/32,
					                       Screen.height*4/32), "Prepare For Mutiny"))
					{
						showMutinyCards = true;
						showOptions = false;
					}
				}
				else
				{
					GUI.Box(new Rect(Screen.width*1/32,
					                 Screen.height*17/32,
					                 Screen.width*6/32,
					                 Screen.height*4/32), "\nPrepare For Mutiny");
					
				}
				
				GUI.Box(makeRect(2,27,4,4), "<size=14><b>" + cards.getCardTitle(mutinyCard1) + "</b></size>" 
				        + "\n" + cards.getCardType(mutinyCard1));
				GUI.Box(makeRect(7,27,4,4), "<size=14><b>" + cards.getCardTitle(mutinyCard2) + "</b></size>" 
				        + "\n" + cards.getCardType(mutinyCard2));
				GUI.Box(makeRect(12,27,4,4), "<size=14><b>" + cards.getCardTitle(mutinyCard3) + "</b></size>" 
				        + "\n" + cards.getCardType(mutinyCard3));
			}
		}


		if (showCaptainCard == true) {
			if(GUI.Button(makeRect(12, 3, 7, 17), " ", updateStyle(cards.getCCardTexture(displayCaptainCard))))
			{
				cards.playCaptainCard(displayCaptainCard);
				if(numOfCapCards == 1) { //follow normal execution
					showOptions = true;
					showCaptainCard = false;
					changePlayer();
				}
				else { //playing multiple cards this turn
					numOfCapCards--;
					drawCaptainCard();
					showOptions = false;
					showCaptainCard = true;
				}
			}
		}




		if (showCards == true)
		{
			if (cardDisplaysArray[0] == "")
			{
				GUI.Box(makeRect(1,1,5,15), "\n" + cardDisplaysArray[0]);
			}
			else if(GUI.Button(makeRect(1,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[0]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[0]);
				cardsArray[0] = 0;
				ps.addPlays(-1);
			}

			/*else if(GUI.Button(new Rect(Screen.width*4/32,
			                            Screen.height*3/32,
			                            Screen.width*4/32,
			                            Screen.height*12/32), "<size=14><b>" + cards.getCardTitle(cardsArray[0]) + "</b></size>" 
			                   + "\n" + cards.getCardType(cardsArray[0]) 
			                   + "\n\n<i>" + cards.getCardDescription(cardsArray[0]) + "</i>", buttonstyle))
			*/
			
			if (cardDisplaysArray[1] == "")
			{
				GUI.Box(makeRect(7,1,5,15), "\n" + cardDisplaysArray[1]);
			}
			else if(GUI.Button(makeRect(7,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[1]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[1]);
				cardsArray[1] = 0;
				ps.addPlays(-1);
			}

			if (cardDisplaysArray[2] == "")
			{
				GUI.Box(makeRect(13,1,5,15), "\n" + cardDisplaysArray[2]);
			}
			else if(GUI.Button(makeRect(13,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[2]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[2]);
				cardsArray[2] = 0;
				ps.addPlays(-1);
			}


			
			if (cardDisplaysArray[3] == "")
			{
				GUI.Box(makeRect(19,1,5,15), "\n" + cardDisplaysArray[3]);
			}
			else if(GUI.Button(makeRect(19,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[3]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[3]);
				cardsArray[3] = 0;
				ps.addPlays(-1);
			}

			
			if (cardDisplaysArray[4] == "")
			{GUI.Box(makeRect(25,1,5,15), "\n" + cardDisplaysArray[4]);}
			else if(GUI.Button(makeRect(25,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[4]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[4]);
				cardsArray[4] = 0;
				ps.addPlays(-1);
			}
			

			if (cardDisplaysArray[5] == "")
			{GUI.Box(makeRect(1,17,5,15), "\n" + cardDisplaysArray[5]);}
			else if(GUI.Button(makeRect(1,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[5]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[5]);
				cardsArray[5] = 0;
				ps.addPlays(-1);
			}

			
			if (cardDisplaysArray[6] == "")
			{GUI.Box(makeRect(7,17,5,15), "\n" + cardDisplaysArray[6]);}
			else if(GUI.Button(makeRect(7,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[6]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[6]);
				cardsArray[6] = 0;
				ps.addPlays(-1);
			}
		
			
			if (cardDisplaysArray[7] == "")
			{GUI.Box(makeRect(13,17,5,15), "\n" + cardDisplaysArray[7]);}
			else if(GUI.Button(makeRect(13,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[7]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[7]);
				cardsArray[7] = 0;
				ps.addPlays(-1);
			}
	
			
			if (cardDisplaysArray[8] == "")
			{GUI.Box(makeRect(19,17,5,15), "\n" + cardDisplaysArray[8]);}
			else if(GUI.Button(makeRect(19,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[8]))))
			{
				showOptions = true;
				showCards = false;
				cards.playCard(cardsArray[8]);
				cardsArray[8] = 0;
				ps.addPlays(-1);;
			}
			
			
			if (cardDisplaysArray[9] == "")
			{GUI.Box(makeRect(25,17,5,15), "\n" + cardDisplaysArray[9]);}
			else if(GUI.Button(makeRect(25,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[9]))))
			{
				showOptions = true;
				showChatWindow = true;
				showCards = false;
				cards.playCard(cardsArray[9]);
				cardsArray[9] = 0;
				ps.addPlays(-1);
			}
		}//End of If(showCards)


		if (showMutinyCards == true)
		{
			if(GUI.Button(makeRect(14,27,4,4), "<size=14><b>" + "Back" + "</b></size>", buttonstyle))
			{
				showOptions = true;
				showMutinyCards = false;
			}
			
			if(ps.getCurrentSpace() == 4 && cards.getCardType(cardsArray[0]) == "Weapon")
			{
				if(GUI.Button(makeRect(4,3,4,12), "<size=14><b>" + cards.getCardTitle(cardsArray[0]) + "</b></size>" 
				              + "\n" + cards.getCardType(cardsArray[0]) 
				              + "\n\n<i>" + cards.getCardDescription(cardsArray[0]) + "</i>", buttonstyle))
				{
					mutinyCard1 = cardsArray[0];
					showOptions = true;
					showMutinyCards = false;
					cardsArray[0] = 0;
					ps.addPlays(-1);
					
				}
			}
			else if(ps.getCurrentSpace() == 10 && cards.getCardType(cardsArray[0]) == "Restraint")
			{
				if(GUI.Button(makeRect(4,3,4,12), "<size=14><b>" + cards.getCardTitle(cardsArray[0]) + "</b></size>" 
				              + "\n" + cards.getCardType(cardsArray[0]) 
				              + "\n\n<i>" + cards.getCardDescription(cardsArray[0]) + "</i>", buttonstyle))
				{
					mutinyCard2 = cardsArray[0];
					showOptions = true;
					showMutinyCards = false;
					cardsArray[0] = 0;
					ps.addPlays(-1);
					
				}
			}
			else if(ps.getCurrentSpace() == 13 && cards.getCardType(cardsArray[0]) == "Clue")
			{
				if(GUI.Button(makeRect(4,3,4,12), "<size=14><b>" + cards.getCardTitle(cardsArray[0]) + "</b></size>" 
				              + "\n" + cards.getCardType(cardsArray[0]) 
				              + "\n\n<i>" + cards.getCardDescription(cardsArray[0]) + "</i>", buttonstyle))
				{
					mutinyCard3 = cardsArray[0];
					showOptions = true;
					showMutinyCards = false;
					cardsArray[0] = 0;
					ps.addPlays(-1);
					
				}
			}
			else
			{
				GUI.Box(makeRect(4,3,4,12), "<size=14><b>" + cards.getCardTitle(cardsArray[0]) + "</b></size>" 
				        + "\n" + cards.getCardType(cardsArray[0]) 
				        + "\n\n<i>" + cards.getCardDescription(cardsArray[0]) + "</i>");
			}
		}



		if (showChoosePlayer == true)
		{
			if(playerTurn == 1)
			{GUI.Box(makeRect(9,10,1,2), "1");}
			else if (GUI.Button(makeRect(9,10,1,2), "1"))
			{
				targetPlayer = 1;
				targetPlayerMethod(targetMethod,targetPlayer);
				showChoosePlayer = false;
			}
			
			if(playerTurn == 2)
			{GUI.Box(makeRect(10,10,1,2), "2");}
			else if (GUI.Button(makeRect(10,10,1,2), "2"))
			{
				targetPlayer = 2;
				targetPlayerMethod(targetMethod,targetPlayer);
				showChoosePlayer = false;
			}
			
			if(StartMenu.numberOfPlayers > 2)
			{
				if(playerTurn == 3)
				{GUI.Box(makeRect(11,10,1,2), "3");}
				else if (GUI.Button(makeRect(11,10,1,2), "3"))
				{
					targetPlayer = 3;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
					{
						useMethod = false;
						targetPlayerMethod(8,playerTurn);
					}
				}
			}
			
			if(StartMenu.numberOfPlayers > 3)
			{
				if(playerTurn == 4)
				{GUI.Box(makeRect(12,10,1,2), "4");}
				else if (GUI.Button(makeRect(12,10,1,2), "4"))
				{
					targetPlayer = 4;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
				}
			}
			
			if(StartMenu.numberOfPlayers > 4)
			{
				if(playerTurn == 5)
				{GUI.Box(makeRect(13,10,1,2), "5");}
				else if (GUI.Button(makeRect(13,10,1,2), "5"))
				{
					targetPlayer = 5;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
				}
			}
			
			if(StartMenu.numberOfPlayers > 5)
			{
				if(playerTurn == 6)
				{GUI.Box(makeRect(14,10,1,2), "6");}
				else if (GUI.Button(makeRect(14,10,1,2), "6"))
				{
					targetPlayer = 6;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
				}
			}
		}


		//commented out at the moment because I was having issue integrating the code!
		if (showChatWindow == true)
		{
			GUILayout.BeginArea(makeRect (22,20,10,10));
			chatScroll = GUILayout.BeginScrollView(chatScroll);
			GUI.skin.box.wordWrap = true;
			GUI.skin.box.alignment = TextAnchor.UpperLeft;
			GUILayout.Box(chatText);
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && fieldText.Length > 0){
				Comm.sendChat(fieldText);
				fieldText = "";
			}
			fieldText = GUI.TextField(makeRect(22,30,10,2), fieldText);
		}

		if(showPathChoice[0] == true)
		{
			string buttonChar = "";
			
			buttonChar = checkForPlayer(1);
			if(showPathChoice[1] == false)
			{
				GUI.Box(makeRect(9,10,1,2), buttonChar, offTile);
			}
			else if (GUI.Button(makeRect(9,10,1,2), buttonChar, startTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(1);
			}
			
			buttonChar = checkForPlayer(2);
			if(showPathChoice[2] == false)
			{
				GUI.Box(makeRect(10,10,1,2), buttonChar, offTile);
			}
			else if (GUI.Button(makeRect(10,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(2);
			}
			
			buttonChar = checkForPlayer(3);
			if(showPathChoice[3] == false)
			{GUI.Box(makeRect(11,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(11,10,1,2), buttonChar,generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(3);
			}
			
			buttonChar = checkForPlayer(4);
			if(showPathChoice[4] == false)
			{GUI.Box(makeRect(12,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(12,10,1,2), buttonChar, weaponTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(4);
			}
			
			buttonChar = checkForPlayer(5);
			if(showPathChoice[5] == false)
			{GUI.Box(makeRect(13,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(13,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(5);
			}
			
			buttonChar = checkForPlayer(6);
			if(showPathChoice[6] == false)
			{GUI.Box(makeRect(14,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(14,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(6);
			}
			
			buttonChar = checkForPlayer(7);
			if(showPathChoice[7] == false)
			{GUI.Box(makeRect(15,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(15,10,1,2), buttonChar, plankTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(7);
			}
			
			buttonChar = checkForPlayer(8);
			if(showPathChoice[8] == false)
			{GUI.Box(makeRect(16,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(16,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(8);
			}
			
			buttonChar = checkForPlayer(9);
			if(showPathChoice[9] == false)
			{GUI.Box(makeRect(17,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(17,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(9);
			}
			
			buttonChar = checkForPlayer(10);
			if(showPathChoice[10] == false)
			{GUI.Box(makeRect(18,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(18,10,1,2), buttonChar, restraintTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(10);
			}
			
			buttonChar = checkForPlayer(11);
			if(showPathChoice[11] == false)
			{GUI.Box(makeRect(19,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(19,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(11);
			}
			
			buttonChar = checkForPlayer(12);
			if(showPathChoice[12] == false)
			{GUI.Box(makeRect(20,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(20,10,1,2), buttonChar, generalTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(12);
			}
			
			buttonChar = checkForPlayer(13);
			if(showPathChoice[13] == false)
			{GUI.Box(makeRect(21,10,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(21,10,1,2), buttonChar, hintTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(13);
			}
			
			buttonChar = checkForPlayer(14);
			if(showPathChoice[14] == false)
			{GUI.Box(makeRect(15,12,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(15,12,1,2), buttonChar, plankTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(14);
			}
			
			buttonChar = checkForPlayer(15);
			if(showPathChoice[15] == false)
			{GUI.Box(makeRect(15,14,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(15,14,1,2), buttonChar, plankTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(15);
			}
			
			buttonChar = checkForPlayer(16);
			if(showPathChoice[16] == false)
			{GUI.Box(makeRect(15,16,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(15,16,1,2), buttonChar, plankTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(16);
			}
		}
	}


	//Helper method
	//Checks to see if the current player is on a specific tile
	public string checkForPlayer(int location){
		PlayerStats temp;
		GameObject temp2;
		string players = "";

		for (int i = 1; i <= 6; i++) {
			temp2 = GameObject.Find ("/Pirate_Ship/Players/Player_" + i) as GameObject;
			temp = temp2.GetComponent ("PlayerStats")as PlayerStats;
			if(temp.getCurrentSpace() == location)
				players = players + " P" + i + " ";
		}
		return players;

	}

	private Rect makeRect(int startWidth, int startHeight, int width, int height)
	{
		return new Rect(Screen.width*startWidth/32,
		                Screen.height*startHeight/32,
		                Screen.width*width/32,
		                Screen.height*height/32); 
	}

	public GUIStyle updateStyle(string newtext){
		Texture2D texture = Resources.Load(newtext) as Texture2D;
		buttonstyle.normal.background = texture;
		buttonstyle.hover.background = texture;
		buttonstyle.active.background = texture;

		return buttonstyle; //just used for holder 
		}
	
}