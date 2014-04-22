using UnityEngine;
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
	public static bool showOpenButton;
	public static bool showMutinyCards;
	public static bool showCurrentMutiny;
	public static bool[] showPathChoice = new bool[18];//0 = turn on display, 1-13 = horizontal, 14-16 = plank, 17 = overboard
	public static bool[] playersAlive = new bool[6];
	public static int playerTurn;
	public static int playerLoseTurn;
	public static int targetPlayer;	
	public static int targetMethod;
	public static int targetAmount;
	public static int round;
	public static int numOfCapCards;
	public static bool useMethod;
	public static int mutinyCard1 = 0;
	public static int mutinyCard2 = 0;
	public static int mutinyCard3 = 0;
	public int cardRangeMin = 1;
	public int cardRangeMax = 27;
	public int captainCardRangeMin = 1;
	public int captainCardRangeMax = 28;
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
	private bool endGame;
	
	public static bool mutinyHelperChoice; //true for placing, false for discarding
	public static bool wait;
	
	public string chatText, fieldText;
	private Vector2 chatScroll = new Vector2(0, 0);
	
	private GameObject tempPlayer;
	private PlayerStats tempStats;
	
	public GUIStyle buttonstyle;
	
	//For generating tile selection for cards
	public GUIStyle generalTile;
	public GUIStyle offTile;
	public GUIStyle startTile;
	public GUIStyle weaponTile;
	public GUIStyle restraintTile;
	public GUIStyle hintTile;
	public GUIStyle plankTile;
	public GUIStyle overboardTile;

	//customization for players
	public GUIStyle player1;
	public GUIStyle player2;
	public GUIStyle player3;
	public GUIStyle player4;
	public GUIStyle player5;
	public GUIStyle player6;

	//backs of cards
	public GUIStyle captainCard;
	public GUIStyle playerCard;
	
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
		showOpenButton = false;
		showChoosePlayer = false;
		showCurrentMutiny = false;
		stealCoins = false;
		useMethod = false;
		populateHand = false;
		numOfCapCards = 1;
		showMutinyCards = false;
		startOfTurn = false;
		endGame = false;
		wait = false;
		
		//Comm.init ();
		Comm.addChatListener (str => {chatText += str+"\n"; chatScroll = new Vector2(chatScroll.x, Mathf.Infinity);});
		Comm.addNewUserListener (str => {chatText += "Player "+str+" has joined the game\n"; chatScroll = new Vector2(chatScroll.x, Mathf.Infinity);});
		//Comm.joinGroup ("renar", "foobar");
	}
	
	/*void Update()
	{
		Debug.Log ("Players alive are, " 
		           + playersAlive[0].ToString() 
		           + ", " + playersAlive[1].ToString() 
		           + ", " + playersAlive[2].ToString()
		           + ", " + playersAlive[3].ToString()
		           + ", " + playersAlive[4].ToString()
		           + ", " + playersAlive[5].ToString());
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

		int aliveCount = 0;
		for(int i = 0; i < StartMenu.numberOfPlayers; i++)
		{
			if(playersAlive[i] == false)
			{
				aliveCount++;
				//Debug.Log("Number of dead players is: " + aliveCount);
			}
		}
		if(aliveCount == StartMenu.numberOfPlayers)
		{
			Debug.Log ("All players are dead");
			//ends the game
			PlayerTurnMenu.showOptions = false;//closes the interface
			PlayerTurnMenu.showCards = false;//closes the card displays
			PlayerTurnMenu.showChatWindow = false;//closes chat box
			StashCount.showLoseGame = true;//TODO needs to display everyone died instead of stash ran out...
			mutinyCard1 = 0; mutinyCard2 = 0; mutinyCard3 = 0;
			StashCount.stashArray[0] = 1;
		}
		else if(aliveCount == StartMenu.numberOfPlayers - 1)
		{
			Debug.Log ("Player " + playerTurn +" is the last player standing\nThey get all the stash!");
			StashCount.stashArray[playerTurn] +=  Mathf.Round(StashCount.stashArray[0]/2);
			//ends the game
			PlayerTurnMenu.showOptions = false;//closes the interface
			PlayerTurnMenu.showCards = false;//closes the card displays
			PlayerTurnMenu.showChatWindow = false;//closes chat box
			StashCount.showLoseGame = true;//TODO needs to display everyone died instead of stash ran out...
			mutinyCard1 = 0; mutinyCard2 = 0; mutinyCard3 = 0;
			StashCount.stashArray[0] = 1;
		}
		else if(currentPlayer == null){changePlayer();}
		else{
			ps = currentPlayer.GetComponent("PlayerStats") as PlayerStats;
			ps.addPlays(2);
			AddCardsToHand(0);
			(Camera.main.GetComponent("FollowPlayer")as FollowPlayer).UpdateCamera(playerTurn);
			//Debug.Log ("ps = " + ps.ToString());
		}

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

	//seperate, used for captain cards (simplier than the normal function)
	//also known as I don't want to break the other AddCardsToHand
	//so I'm saying fuck it to good coding practice
	public void AddCardsToHand2(int add, int player)
	{
		float loop = player;
		
		for(int j = player; j <= loop; j++)
		{
			ps = (GameObject.Find("/Pirate_Ship/Players/Player_" + player)).GetComponent("PlayerStats") as PlayerStats;
			cardsArray = ps.getCards();
			ps.addCards(add);
			for(int i = 0; i < cardsArray.Length;i++)
			{
				if(cardsArray[i] == 0 && ps.getCardsToAdd() > 0)
				{
					randomCard = Random.Range(cardRangeMin,cardRangeMax+1);
					randomCard = (int) Mathf.Round(randomCard);
					cardsArray[i] = randomCard;
					ps.addCards(-1);
				}
				if(cardsArray[i] == 7 && ps.getDrawRopes() == false)
				{
					cardsArray[i] = 1;
				}
			}
			ps.setCards(cardsArray);// update player hand
		}
		ps = (GameObject.Find("/Pirate_Ship/Players/Player_" + playerTurn)).GetComponent("PlayerStats") as PlayerStats;
	}
	
	//probably needs to be handled in some way for drawing multiple Captain cards
	//Though players don't get to choose order...
	//so captain cards should still only come up one at a time?
	public void drawCaptainCard(){
		randomCard = Random.Range(captainCardRangeMin,captainCardRangeMax+1); //1 to 28
		randomCard = (int) Mathf.Round(randomCard);
		displayCaptainCard = randomCard;
		//displayCaptainCard = 6; //test specific cards
		Debug.Log("Captain card is " + displayCaptainCard );
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
			
			if (current + spaces < 17)
			{
				endPlank = current + spaces;
			}
			else
			{
				endPlank = 17;
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
				if (endPlank > 17)
				{endPlank = 17;}
				
				for(int k = 14; k <= endPlank; k++)
				{showPathChoice[k] = true;}
			}
			else if (current - spaces < 7 && current > 7)
			{
				endPlank = (7 - (current - spaces)) + 13;
				if (endPlank > 17)
				{endPlank = 17;}
				
				for(int k = 14; k <= endPlank; k++)
				{showPathChoice[k] = true;}
			}
		}
		showPathChoice[0] = true;
	}
	public void movePlayerHelper(int space)
	{
		GameObject playOb;
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
		for(int k = 1; k <= 17; k++)
		{showPathChoice[k] = false;}
		if(useMethod == true)
		{
			useMethod = false;
			targetPlayerMethod(8,playerTurn);
		}
		if(ps2.getCurrentSpace() == 17)
		{
			playersAlive[ps2.getPlayerID()-1] = false;
			playOb = (GameObject.Find("/Pirate_Ship/Players/Player_" + ps2.getPlayerID()) as GameObject);
			playOb.SetActive(false);
		}
		else
		{
			(Camera.main.GetComponent("FollowPlayer")as FollowPlayer).UpdateCamera(playerTurn);
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

		if(tempStats.getCurrentSpace() == 17)
		{
			playersAlive[tempStats.getPlayerID()-1] = false;
			tempPlayer = (GameObject.Find("/Pirate_Ship/Players/Player_" + tempStats.getPlayerID()) as GameObject);
			tempPlayer.SetActive(false);
		}
		else
		{
			(Camera.main.GetComponent("FollowPlayer")as FollowPlayer).UpdateCamera(playerTurn);
		}
	}
	
	public void moveTowardOverboard(int player,int spaces)
	{
		tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + player);
		if(tempPlayer != null){
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
					current = 17;
			}
			
			//finally, actually move the player
			changePlayerLocation (player, current); //at this point, current is actually the new location
			/*if(tempStats.getCurrentSpace() == 17)
		{
			playersAlive[tempStats.getPlayerID()] = false;
			(GameObject.Find("/Pirate_Ship/Players/Player_" + tempStats.getPlayerID()) as GameObject).SetActive(false);
		}*/
		}

	}
	
	
	void OnGUI()
	{
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;

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

				GUI.Box(makeRect(1,1,4,4), "Player " + playerTurn, currentPlayerStyle());
				GUI.Box(makeRect(6,1,3,4), "Cards: " + ps.getPlays());
				GUI.Box(makeRect(10,1,3,4), "Round " + round);
				
				if (ps.getPlays() > 0f)
				{
					GUI.Box(makeRect(1,6,4,4), "End Turn");
				}
				else
				{
					if(GUI.Button(makeRect(1,6,4,4), "End Turn")) 
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
					GUI.Box(makeRect(1,11,4,4), "Play A Card");
				}
				else
				{
					if(GUI.Button(makeRect(1,11,4,4), "Play A Card")) 
					{
						//Debug.Log("Inside Play A card button");
						///Debug.Log("Number of cards to play: " + ps.getCardsToAdd());
						//Debug.Log(optionString);
						if (ps.getPlays() > 0)
						{
							AddCardsToHand(0);
							showOptions = false;
							showChatWindow = false;
							showCards = true;
						}
					}
				}
				
				if (ps.getPlays() > 0f && (ps.getCurrentSpace() == 4 || ps.getCurrentSpace() == 10 || ps.getCurrentSpace() == 13 || ps.getCurrentSpace() == 7))//need to check if they are on the correct space
				{
					if(GUI.Button(makeRect(1,16,4,4), "Prepare For Mutiny"))
					{
						if(mutinyCard1 != 0 && mutinyCard2 != 0 && mutinyCard3 != 0){
							//doing mutiny!
							StashCount.mutiny = true;
							
							//deciding winner
							//dueling? TODO later if we want...
							//Current defaulting to "Split the Loot"
							//extra goes to whoever started the mutiny
							while(StashCount.stashArray[0] >= StartMenu.numberOfPlayers){
								for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
									StashCount.stashArray[0] -= 1;
									StashCount.stashArray[i] += 1;
								}
							}
							StashCount.stashArray[playerTurn] += StashCount.stashArray[0];
							
							//ends the game
							PlayerTurnMenu.showOptions = false;//closes the interface
							PlayerTurnMenu.showCards = false;//closes the card displays
							PlayerTurnMenu.showChatWindow = false;//closes chat box
							endGame = true;//opens the lose game display
							mutinyCard1 = 0; mutinyCard2 = 0; mutinyCard3 = 0;
							StashCount.stashArray[0] = 1;
						}
						else { //just playing a mutiny card
							showMutinyCards = true;
							showChatWindow = false;
							showOptions = false;
						}
					}
				}
				else
				{
					GUI.Box(makeRect(1,16,4,4), "Prepare For Mutiny");
				}
				
				if(mutinyCard1 < 100 && mutinyCard1 != 0) {
					GUI.Box(makeRect(1,27,4,4), " ", playerCard);
				}
				else if(mutinyCard1 > 100 && mutinyCard1 != 0){
					GUI.Box(makeRect(1,27,4,4), " ", captainCard);
				}
				else {
					GUI.Box(makeRect(1,27,4,4), " ");
				}
				if(mutinyCard2 < 100 && mutinyCard2 != 0) {
					GUI.Box(makeRect(6,27,4,4), " ", playerCard);
				}
				else if(mutinyCard2 > 100 && mutinyCard2 != 0){
					GUI.Box(makeRect(6,27,4,4), " ", captainCard);
				}
				else {
					GUI.Box(makeRect(6,27,4,4), " ");
				}
				if(mutinyCard3 < 100 && mutinyCard3 != 0) {
					GUI.Box(makeRect(11,27,4,4), " ", playerCard);
				}
				else if(mutinyCard3 > 100 && mutinyCard3 != 0){
					GUI.Box(makeRect(11,27,4,4), " ", captainCard);
				}
				else {
					GUI.Box(makeRect(11,27,4,4), " ");
				}
			}

			if(showOpenButton == true){
				if(GUI.Button(makeRect(29, 30, 3, 2), "Open Chat")){
					showChatWindow = true;
					showOpenButton = false;
				}
			}

			//Moved stash boxes from StashCount - made styling easier
			//Captain Stash
			GUI.Box(makeRect(14,1,6,4), "Captain's Gold Stash: " + StashCount.stashArray[0]);
			
			//Player 1 Stash
			GUI.Box(makeRect(26, 1, 6, 2), "Player One's Gold Stash: " + StashCount.stashArray[1], player1);
			
			if(StartMenu.numberOfPlayers > 1)
			{
				//Player 2 Stash
				GUI.Box(makeRect(26, 4, 6, 2), "Player Two's Gold Stash: " + StashCount.stashArray[2], player2);
			}
			if(StartMenu.numberOfPlayers > 2)
			{
				//Player 3 Stash
				GUI.Box(makeRect(26, 7, 6, 2), "Player Three's Gold Stash: " + StashCount.stashArray[3], player3);
			}
			if(StartMenu.numberOfPlayers > 3)
			{
				//Player 4 Stash
				GUI.Box(makeRect(26, 10, 6, 2), "Player Four's Gold Stash: " + StashCount.stashArray[4], player4);
			}
			if(StartMenu.numberOfPlayers > 4)
			{
				//Player 5 Stash
				GUI.Box(makeRect(26, 13, 6, 2), "Player Five's Gold Stash: " + StashCount.stashArray[5], player5);
			}
			if(StartMenu.numberOfPlayers > 5)
			{
				//Player 6 Stash
				GUI.Box(makeRect(26, 16, 6, 2), "Player Six's Gold Stash: " + StashCount.stashArray[6], player6);
			}
		}
		
		
		if (showCaptainCard == true) {
			if(GUI.Button(makeRect(12, 3, 7, 17), " ", updateStyle(cards.getCCardTexture(displayCaptainCard))))
			{
				cards.playCaptainCard(displayCaptainCard);
				if(numOfCapCards == 1) { //follow normal execution
					if(!wait){
						showOptions = true;
						showCaptainCard = false;
						changePlayer();
					}
				}
				else { //playing multiple cards this turn
					if(!wait){
						numOfCapCards--;
						drawCaptainCard();
						showOptions = false;
						showCaptainCard = true;
					}
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
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
				if(showOpenButton != true)
					showChatWindow = true;
				showCards = false;
				cards.playCard(cardsArray[9]);
				cardsArray[9] = 0;
				ps.addPlays(-1);
			}
		}//End of If(showCards)
		
		if (showChoosePlayer == true)
		{
			if(playerTurn == 1 || playersAlive[0] != true)
			{GUI.Box(makeRect(9,10,1,2), "1");}
			else if (GUI.Button(makeRect(9,10,1,2), "1", player1))
			{
				targetPlayer = 1;
				targetPlayerMethod(targetMethod,targetPlayer);
				showChoosePlayer = false;
			}
			
			if(playerTurn == 2 || playersAlive[1] != true)
			{GUI.Box(makeRect(10,10,1,2), "2");}
			else if (GUI.Button(makeRect(10,10,1,2), "2", player2))
			{
				targetPlayer = 2;
				targetPlayerMethod(targetMethod,targetPlayer);
				showChoosePlayer = false;
			}
			
			if(StartMenu.numberOfPlayers > 2)
			{
				if(playerTurn == 3 || playersAlive[2] != true)
				{GUI.Box(makeRect(11,10,1,2), "3");}
				else if (GUI.Button(makeRect(11,10,1,2), "3", player3))
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
				if(playerTurn == 4 || playersAlive[3] != true)
				{GUI.Box(makeRect(12,10,1,2), "4");}
				else if (GUI.Button(makeRect(12,10,1,2), "4", player4))
				{
					targetPlayer = 4;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
				}
			}
			
			if(StartMenu.numberOfPlayers > 4)
			{
				if(playerTurn == 5 || playersAlive[4] != true)
				{GUI.Box(makeRect(13,10,1,2), "5");}
				else if (GUI.Button(makeRect(13,10,1,2), "5", player5))
				{
					targetPlayer = 5;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
				}
			}
			
			if(StartMenu.numberOfPlayers > 5)
			{
				if(playerTurn == 6 || playersAlive[5] != true)
				{GUI.Box(makeRect(14,10,1,2), "6");}
				else if (GUI.Button(makeRect(14,10,1,2), "6", player6))
				{
					targetPlayer = 6;
					targetPlayerMethod(targetMethod,targetPlayer);
					showChoosePlayer = false;
				}
			}
		}
		
		if (showChatWindow == true)
		{
			if(GUI.Button(makeRect(29, 19, 3, 2), "Hide Chat")){
				showChatWindow = false;
				showOpenButton = true;
			}
			GUILayout.BeginArea(makeRect (22,21,10,9));
			chatScroll = GUILayout.BeginScrollView(chatScroll);
			GUI.skin.box.wordWrap = true;
			GUI.skin.box.alignment = TextAnchor.UpperLeft;
			GUILayout.Box(chatText, GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && fieldText.Length > 0){
				Comm.sendChat(fieldText);
				fieldText = "";
			}
			fieldText = GUI.TextField(makeRect(22,30,10,2), fieldText);
		}
		
		if(endGame == true)
		{
			if(round > 2)
			{
				if(GUI.Button(makeRect(10,12,12,8), "Mutiny" + "\nThat was the last round, start a new game?"))
				{
					endGame = false;
					Application.LoadLevel("Intermission");//Start the level over
					StartMenu.inputServerString = "";
					StartMenu.inputPasswordString = "";
					StartMenu.showStartMenu = true;//show main menu box
					StartMenu.showMakeGame = true;//show host/join menu
					StartMenu.showHostMenu = false;//Don't show host menu
					StartMenu.showJoinMenu = false;//Don't show join menu
				}
			}
			else 
			{
				string winningPlayer = checkWinningPlayer();
				if(GUI.Button(makeRect(10,10,12,8), "Player " + playerTurn + 
				              " has started the Mutiny!\n\n" + 
				              winningPlayer + "\n\n" +
				              "\nStart next round?"))
				{
					Application.LoadLevel("Intermission");//Start the level over
					endGame = false;//closes the lose interface
					StashCount.stashArray[0] = StashCount.captainStashMultiplier * StartMenu.numberOfPlayers;
					for(int i = 1; i < StashCount.stashArray.Length;i++)//Adds more stash at beginning of round
					{StashCount.stashArray[i] += StashCount.stash;}
					Application.LoadLevel("InGame");//Start the level over
					showOptions = true;//opens the game interface
					showChatWindow = true;
					showOpenButton = false;
					playerTurn = 0;
				}
			}
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
			
			buttonChar = checkForPlayer(17);
			if(showPathChoice[17] == false)
			{GUI.Box(makeRect(15,18,1,2), buttonChar, offTile);}
			else if (GUI.Button(makeRect(15,18,1,2), buttonChar, overboardTile))
			{
				showPathChoice[0] = false;
				movePlayerHelper(17);
			}
		}
		
		if(showCurrentMutiny == true){
			if(!mutinyHelperChoice && mutinyCard1 == 0){ //discard
				{GUI.Box(makeRect(1,27,4,4), " ");}
			}
			else if(mutinyCard1 == 0){
				if(GUI.Button(makeRect(1,27,4,4), " " ))
				{
					mutinyCCPlace(1);
				}
			}
			else if(mutinyCard1 < 100) {
				if(GUI.Button(makeRect(1,27,4,4), " ", playerCard))
				{
					mutinyCaptainCard(1);
				}
			}
			else {
				if(GUI.Button(makeRect(1,27,4,4), " ", captainCard))
				{
					mutinyCaptainCard(1);
				}
			}

			if(!mutinyHelperChoice && mutinyCard2 == 0){ //discard
				{GUI.Box(makeRect(6,27,4,4), " ");}
			}
			else if(mutinyCard2 == 0){
				if(GUI.Button(makeRect(6,27,4,4), " " ))
				{
					mutinyCCPlace(2);
				}
			}
			else if(mutinyCard2 < 100) {
				if(GUI.Button(makeRect(6,27,4,4), " ", playerCard))
				{
					mutinyCaptainCard(2);
				}
			}
			else {
				if(GUI.Button(makeRect(6,27,4,4), " ", captainCard))
				{
					mutinyCaptainCard(2);
				}
			}

			if(!mutinyHelperChoice && mutinyCard3 == 0){ //discard
				{GUI.Box(makeRect(11,27,4,4), " ");}
			}
			else if(mutinyCard3 == 0){
				if(GUI.Button(makeRect(11,27,4,4), " " ))
				{
					mutinyCCPlace(3);
				}
			}
			else if(mutinyCard3 < 100) {
				if(GUI.Button(makeRect(11,27,4,4), " ", playerCard))
				{
					mutinyCaptainCard(3);
				}
			}
			else {
				if(GUI.Button(makeRect(11,27,4,4), " ", captainCard))
				{
					mutinyCaptainCard(3);
				}
			}
		}
		
		//lets players play cards into mutiny slots
		if (showMutinyCards == true)
		{
			if(GUI.Button(makeRect(14,27,4,4), "<size=14><b>" + "Back" + "</b></size>"))
			{
				showOptions = true;
				showMutinyCards = false;
			}
			
			if(ps.getCurrentSpace() == 4)
			{
				if(cards.getCardType(cardsArray[0]) == "Weapon") {
					if(GUI.Button(makeRect(1,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[0]))))
					{ 
						mutinyCard1 = cardsArray[0];
						mutinyCardHelper(0);
					}
				}
				/*else {
					GUI.Box(makeRect(4,3,4,12), " ", updateStyle(cards.getCardTexture(cardsArray[0])));
				}*/
				if(cards.getCardType(cardsArray[1]) == "Weapon") {
					if(GUI.Button(makeRect(7,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[1]))))
					{ 
						mutinyCard1 = cardsArray[1];
						mutinyCardHelper(1);
					}
				}
				if(cards.getCardType(cardsArray[2]) == "Weapon") {
					if(GUI.Button(makeRect(13,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[2]))))
					{ 
						mutinyCard1 = cardsArray[2];
						mutinyCardHelper(2);
					}
				}
				if(cards.getCardType(cardsArray[3]) == "Weapon") {
					if(GUI.Button(makeRect(19,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[3]))))
					{ 
						mutinyCard1 = cardsArray[3];
						mutinyCardHelper(3);
					}
				}
				if(cards.getCardType(cardsArray[4]) == "Weapon") {
					if(GUI.Button(makeRect(25,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[4]))))
					{ 
						mutinyCard1 = cardsArray[4];
						mutinyCardHelper(4);
					}
				}
				if(cards.getCardType(cardsArray[5]) == "Weapon") {
					if(GUI.Button(makeRect(1,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[5]))))
					{ 
						mutinyCard1 = cardsArray[5];
						mutinyCardHelper(5);
					}
				}
				if(cards.getCardType(cardsArray[6]) == "Weapon") {
					if(GUI.Button(makeRect(7,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[6]))))
					{ 
						mutinyCard1 = cardsArray[6];
						mutinyCardHelper(6);
					}
				}
				if(cards.getCardType(cardsArray[7]) == "Weapon") {
					if(GUI.Button(makeRect(13,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[7]))))
					{ 
						mutinyCard1 = cardsArray[7];
						mutinyCardHelper(7);
					}
				}
				if(cards.getCardType(cardsArray[8]) == "Weapon") {
					if(GUI.Button(makeRect(19,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[8]))))
					{ 
						mutinyCard1 = cardsArray[8];
						mutinyCardHelper(8);
					}
				}
				if(cards.getCardType(cardsArray[9]) == "Weapon") {
					if(GUI.Button(makeRect(25,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[9]))))
					{ 
						mutinyCard1 = cardsArray[9];
						mutinyCardHelper(9);
					}
				}
			}
			else if(ps.getCurrentSpace() == 10)
			{
				if(cards.getCardType(cardsArray[0]) == "Restraint") {
					if(GUI.Button(makeRect(1,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[0]))))
					{ 
						mutinyCard2 = cardsArray[0];
						mutinyCardHelper(0);
					}
				}
				if(cards.getCardType(cardsArray[1]) == "Restraint") {
					if(GUI.Button(makeRect(7,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[1]))))
					{ 
						mutinyCard2 = cardsArray[1];
						mutinyCardHelper(1);
					}
				}
				if(cards.getCardType(cardsArray[2]) == "Restraint") {
					if(GUI.Button(makeRect(13,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[2]))))
					{ 
						mutinyCard2 = cardsArray[2];
						mutinyCardHelper(2);
					}
				}
				if(cards.getCardType(cardsArray[3]) == "Restraint") {
					if(GUI.Button(makeRect(19,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[3]))))
					{ 
						mutinyCard2 = cardsArray[3];
						mutinyCardHelper(3);
					}
				}
				if(cards.getCardType(cardsArray[4]) == "Restraint") {
					if(GUI.Button(makeRect(25,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[4]))))
					{ 
						mutinyCard2 = cardsArray[4];
						mutinyCardHelper(4);
					}
				}
				if(cards.getCardType(cardsArray[5]) == "Restraint") {
					if(GUI.Button(makeRect(1,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[5]))))
					{ 
						mutinyCard2 = cardsArray[5];
						mutinyCardHelper(5);
					}
				}
				if(cards.getCardType(cardsArray[6]) == "Restraint") {
					if(GUI.Button(makeRect(7,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[6]))))
					{ 
						mutinyCard2 = cardsArray[6];
						mutinyCardHelper(6);
					}
				}
				if(cards.getCardType(cardsArray[7]) == "Restraint") {
					if(GUI.Button(makeRect(13,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[7]))))
					{ 
						mutinyCard2 = cardsArray[7];
						mutinyCardHelper(7);
					}
				}
				if(cards.getCardType(cardsArray[8]) == "Restraint") {
					if(GUI.Button(makeRect(19,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[8]))))
					{ 
						mutinyCard2 = cardsArray[8];
						mutinyCardHelper(8);
					}
				}
				if(cards.getCardType(cardsArray[9]) == "Restraint") {
					if(GUI.Button(makeRect(25,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[9]))))
					{ 
						mutinyCard2 = cardsArray[9];
						mutinyCardHelper(9);
					}
				}
			}
			else if(ps.getCurrentSpace() == 13)
			{
				if(cards.getCardType(cardsArray[0]) == "Hint") {
					if(GUI.Button(makeRect(1,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[0]))))
					{ 
						mutinyCard3 = cardsArray[0];
						mutinyCardHelper(0);
					}
				}
				if(cards.getCardType(cardsArray[1]) == "Hint") {
					if(GUI.Button(makeRect(7,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[1]))))
					{ 
						mutinyCard3 = cardsArray[1];
						mutinyCardHelper(1);
					}
				}
				if(cards.getCardType(cardsArray[2]) == "Hint") {
					if(GUI.Button(makeRect(13,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[2]))))
					{ 
						mutinyCard3 = cardsArray[2];
						mutinyCardHelper(2);
					}
				}
				if(cards.getCardType(cardsArray[3]) == "Hint") {
					if(GUI.Button(makeRect(19,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[3]))))
					{ 
						mutinyCard3 = cardsArray[3];
						mutinyCardHelper(3);
					}
				}
				if(cards.getCardType(cardsArray[4]) == "Hint") {
					if(GUI.Button(makeRect(25,1,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[4]))))
					{ 
						mutinyCard3 = cardsArray[4];
						mutinyCardHelper(4);
					}
				}
				if(cards.getCardType(cardsArray[5]) == "Hint") {
					if(GUI.Button(makeRect(1,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[5]))))
					{ 
						mutinyCard3 = cardsArray[5];
						mutinyCardHelper(5);
					}
				}
				if(cards.getCardType(cardsArray[6]) == "Hint") {
					if(GUI.Button(makeRect(7,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[6]))))
					{ 
						mutinyCard3 = cardsArray[6];
						mutinyCardHelper(6);
					}
				}
				if(cards.getCardType(cardsArray[7]) == "Hint") {
					if(GUI.Button(makeRect(13,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[7]))))
					{ 
						mutinyCard3 = cardsArray[7];
						mutinyCardHelper(7);
					}
				}
				if(cards.getCardType(cardsArray[8]) == "Hint") {
					if(GUI.Button(makeRect(19,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[8]))))
					{ 
						mutinyCard3 = cardsArray[8];
						mutinyCardHelper(8);
					}
				}
				if(cards.getCardType(cardsArray[9]) == "Hint") {
					if(GUI.Button(makeRect(25,17,5,15), " ", updateStyle(cards.getCardTexture(cardsArray[9]))))
					{ 
						mutinyCard3 = cardsArray[9];
						mutinyCardHelper(9);
					}
				}
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
			if(temp2 != null){
				temp = temp2.GetComponent ("PlayerStats")as PlayerStats;
				if(temp.getCurrentSpace() == location)
					players = players + " P" + i + " ";
			}
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
	
	public void endTurn(){
		if(numOfCapCards == 1) { //follow normal execution
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
	
	public GUIStyle updateStyle(string newtext){
		Texture2D texture = Resources.Load(newtext) as Texture2D;
		buttonstyle.normal.background = texture;
		buttonstyle.hover.background = texture;
		buttonstyle.active.background = texture;
		
		return buttonstyle; //just used for holder 
	}
	
	public void mutinyCardHelper(int cardnum){
		showOptions = true;
		if(showOpenButton != true)
			showChatWindow = true;
		showMutinyCards = false;
		cardsArray[cardnum] = 0;
		ps.addPlays(-1);
		
		//reward for playing mutiny card
		StashCount.stashArray[playerTurn] += 2;
	}	

	public void mutinyCaptainCard(int card){
		showOptions = true;
		showCurrentMutiny = false;
		wait = false;
		if(mutinyHelperChoice){ //place 
			if(card == 1)
				mutinyCard1 = 102; //this is the only card that sets this (#2)
			else if(card == 2)
				mutinyCard2 = 102;
			else
				mutinyCard3 = 102;
		}
		else {//discard
			if(card == 1)
				mutinyCard1 = 0; //this is the only card that sets this (#2)
			else if(card == 2)
				mutinyCard2 = 0;
			else
				mutinyCard3 = 0;
		}
		
		endTurn();
	}

	public void mutinyCCPlace(int card){
		showOptions = true;
		showCurrentMutiny = false;
		wait = false;
		if(card == 1)
			mutinyCard1 = 102; //this is the only card that sets this (#2)
		else if(card == 2)
			mutinyCard2 = 102;
		else
			mutinyCard3 = 102;

		endTurn();
	}

	//customizing for players turns
	private GUIStyle currentPlayerStyle(){
		if (playerTurn == 1)
			return player1;
		else if (playerTurn == 2)
			return player2;
		else if (playerTurn == 3)
			return player3;
		else if (playerTurn == 4)
			return player4;
		else if (playerTurn == 5)
			return player5;
		else
			return player6;

	}

	//checking which player is currently winning by gold
	public string checkWinningPlayer(){
		float max = StashCount.stashArray [1];
		int playermax = 1;

		for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
			float temp = StashCount.stashArray[i];
			if(temp > max){max = temp; playermax = i;}
		}

		//check for ties
		int count = 0;
		for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
			if(StashCount.stashArray[i] == max)
				count++;
		}

		if(count > 1)
			return "There is a tie!";
		else
			return "Player " + playermax + " is currently winning with " + StashCount.stashArray[playermax] + " gold!";
	}
}