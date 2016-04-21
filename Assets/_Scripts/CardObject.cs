using UnityEngine;
using System.Collections;

//Card object
//need a method for choosing another player for attacks

public class CardObject : MonoBehaviour {
	public int cardNumber; //identifies card number
	private string title; //Title of card
	private string description; //Flavour text
	private string type; //Weapon, Hint, Restraint, Move, General
	private string texture; //used for the card image
	
	private PlayerTurnMenu ptm;
	private GameObject tempPlayer;
	private PlayerStats tempStats;
	
	private float maxNumCoins, minNumCoins, check;
	private int playermax, playermin;
	
	void Start(){
		cardNumber = 0;
		title = "";
		description = "";
		type = "default";
		texture = "none";
		ptm = gameObject.GetComponent("PlayerTurnMenu") as PlayerTurnMenu;
	}
	
	//for player cards
	public void action(PlayerTurnMenu ptm){
		switch(cardNumber)
		{
		case 0:
			Debug.Log("Empty cards don't do anything");
			break;
			
		case 1:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 2;
			PlayerTurnMenu.targetAmount = 1;
			//Steal 1 coin from any player
			//how to
			break;
			
		case 2:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 2;
			PlayerTurnMenu.targetAmount = 2;
			//Steal 2 coins from any player
			//how to (see case 1)
			break;
			
		case 3:
			//move 1 space
			ptm.movePlayer(PlayerTurnMenu.playerTurn,1);
			break;
			
		case 4:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 1;
			PlayerTurnMenu.targetAmount = 1;
			//Move another player one space
			break;
			
		case 5:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 1;
			PlayerTurnMenu.targetAmount = 2;
			//Move another player up to 2 spaces
			break;
			
		case 6:
			//PlayerTurnMenu.targetMethod = 2;
			//PlayerTurnMenu.targetAmount = 2;
			//Move each OPPONENT up 2 spaces towards Overboad
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				if(i != PlayerTurnMenu.playerTurn)
					ptm.moveTowardOverboard(i, 2);
			}
			break;
			
		case 7:
			//rope - save yourself from falling overboard
			//draw a card
			//player goes back to the end of the plank
			//TODO
			break;
			
		case 8:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 3;
			PlayerTurnMenu.targetAmount = -1;
			//Another player only draws and plays 1 card on their next turn
			break;
			
		case 9:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 4;
			PlayerTurnMenu.targetAmount = 0;
			//Pull another player into your space
			break;
			
		case 10:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 5;
			//Force a player to lose their next turn
			break;
			
		case 11:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 4;
			PlayerTurnMenu.targetAmount = 1;
			//Switch places with another player
			break;
			
		case 12:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 6;
			//Target player cannot use rope cards for the rest of the game
			break;
			
		case 13:
			ptm.ps.setPlays(3);
			ptm.ps.addCards(2);
			//Draw 2 card and play them immediately.
			break;
			
		case 14:
			ptm.targetPlayerMethod(7,PlayerTurnMenu.playerTurn);
			//move to any mutiny space
			break;
			
		case 15:
			ptm.ps.setPlays(4);
			ptm.ps.addCards(3);
			//draw 3 cards
			break;
			
		case 16:
			StashCount.stashArray[0] -= 1;
			StashCount.stashArray[PlayerTurnMenu.playerTurn] += 1;
			//steal 1 coins from stash
			break;
			
		case 17:
			ptm.movePlayer(PlayerTurnMenu.playerTurn,3);
			//move 3
			break;
			
		case 18:
			ptm.movePlayer(PlayerTurnMenu.playerTurn,5);
			//move 5
			break;
			
		case 19:
			PlayerTurnMenu.targetAmount = 1;
			ptm.targetPlayerMethod(8,PlayerTurnMenu.playerTurn);
			//Move 1 space
			//Take 1 coin from each opponent in that space
			break;
			
		case 20:
			PlayerTurnMenu.showChoosePlayer = true;
			PlayerTurnMenu.targetMethod = 9;
			PlayerTurnMenu.targetAmount = 1;
			//move an opponent 1 space and steal a coin from them
			break;
			
		case 21:
			//take 2 coins from the stash
			StashCount.stashArray[0] -= 2;
			StashCount.stashArray[PlayerTurnMenu.playerTurn] += 2;
			break;
			
		case 22:
			//Move up to 3 spaces and take up to 1 coin from a player on the way
			//TODO
			break;
			
		case 23:
			PlayerTurnMenu.targetAmount = 1;
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++)
				ptm.targetPlayerMethod(2,i);
			//Steal one coin from each player
			break;
			
		case 24:
			//move 1 space
			//draw 1 card and play immediately
			ptm.movePlayer(PlayerTurnMenu.playerTurn,1);
			ptm.ps.setPlays(1);
			ptm.ps.addCards(1);
			break;
			
		case 25:
			//take 2 coins from the stash
			StashCount.stashArray[0] -= 2;
			StashCount.stashArray[PlayerTurnMenu.playerTurn] += 2;
			//draw a second captain's card this turn 
			PlayerTurnMenu.numOfCapCards = 2;
			break;
			
		case 26:
			ptm.targetPlayerMethod(10,1);
			//take 5 coins from the stash
			//give 1 to each opponent
			//if any leftover give to player
			break;
			
		case 27:
			//Do not draw a Captain's Card this turn
			PlayerTurnMenu.numOfCapCards = 0;
			break;
			
		default:
			Debug.Log("I think you broke it");
			break;
		}
	}
	
	//Actions for Captain cards
	public void actionCpt(PlayerTurnMenu ptm){
		switch(cardNumber)
		{
		case 1:
			//each player must discard any move cards in their hands and draw that many
			discardCards("Move", ptm);
			break;
			
		case 2:
			//all players in a mutiny space must move to the plank
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + i);
				tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
				
				//check to see if player is in mutiny place
				//if so, move to start of plank
				int currentSpace = tempStats.getCurrentSpace();
				if(currentSpace == 4 || currentSpace == 10 || currentSpace == 13){
					ptm.changePlayerLocation(i, 7);
				}
				
			}
			//place this card in any mutiny slot
			PlayerTurnMenu.mutinyHelperChoice = true;
			PlayerTurnMenu.wait = true;
			PlayerTurnMenu.showCurrentMutiny = true;
			PlayerTurnMenu.showOptions = false;
			
			//add 2 coins to the stash
			StashCount.stashArray[0] += 2;
			break;
			
		case 3:
			//the players with the most coins must give 1 coin to the players with the fewest
			//handling ties... take coins from each player with max amount of coins
			//give out taken number of coins to players with the min amount of coins
			maxNumCoins = 0;
			minNumCoins = StashCount.stashArray[1];
			int count = 0;
			
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				float temp = StashCount.stashArray[i];
				if(temp < minNumCoins){minNumCoins = temp;}
				if(temp > maxNumCoins){maxNumCoins = temp;}
			}
			
			if (minNumCoins == maxNumCoins){
				//Debug.Log("Min: " + minNumCoins + "\n Max: " + maxNumCoins);
				return;
			}

			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				if(StashCount.stashArray[i] == maxNumCoins){
					StashCount.stashArray[i] -= 1;
					count++;
				}
			}
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				if(StashCount.stashArray[i] == minNumCoins && count > 0){
					StashCount.stashArray[i] += 1;
					count--;
				}
			}
			break;
			
		case 4:
			//Move the current player 2 spaces towards Overboard!
			ptm.moveTowardOverboard(PlayerTurnMenu.playerTurn, 2);
			break;
			
		case 5:
			//The current player must put 2 coins in the stash
			check = checkStash(PlayerTurnMenu.playerTurn, 2);
			StashCount.stashArray[PlayerTurnMenu.playerTurn] -= check;
			StashCount.stashArray[0] += check;
			break;
			
		case 6:
			//The current player must draw 2 more Captain's Cards this turn
			PlayerTurnMenu.numOfCapCards = 3;
			break;
			
		case 7:
			//The current player must move to the start space
			ptm.changePlayerLocation(PlayerTurnMenu.playerTurn,1);
			break;
			
		case 8:
			//each player may discard any number of cards to draw that many cards
			//TODO
			break;
			
		case 9:
			//The current player must move 4 spaces towards Overboard
			ptm.moveTowardOverboard(PlayerTurnMenu.playerTurn, 4);
			break;
			
		case 10:
			//remove 5 coins from the stash
			StashCount.stashArray[0] -= 5;
			break;
			
		case 11:
			//remove 3 coins from the stash
			StashCount.stashArray[0] -= 3;
			break;
			
		case 12:
			//move each player 2 spaces toward Overboard
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				ptm.moveTowardOverboard(i, 2);
			}
			break;
			
		case 13:
			//remove two coins from the stash
			StashCount.stashArray[0] -= 2;
			break;
			
		case 14:
			//Move each player 4 spaces towards Overboard
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				ptm.moveTowardOverboard(i, 4);
			}
			break;
			
		case 15:
			//Move each player 1 spaces toward Overboard
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				ptm.moveTowardOverboard(i, 1);
			}
			break;
			
		case 16:
			//each player must put 1 coin into the stash
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				check = checkStash(i, 1);
				StashCount.stashArray[i] -= check;
				StashCount.stashArray[0] += check;
			}
			break;
			
		case 17:
			//each player must discard any Restraint cards in their hands
			//and draw that many cards
			discardCards("Restraint", ptm);
			break;
			
		case 18:
			//Each player discards all Weapons cards and draws that many new cards
			discardCards("Weapon", ptm);
			break;
			
		case 19:
			//Each player discards all hint cards and draws that many new cards
			discardCards("Hint", ptm);
			break;
			
		case 20:
			//The player with the most coins must move 3 spaces towards Overboard
			maxNumCoins = 0;
			
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				float temp = StashCount.stashArray[i];
				if(temp > maxNumCoins){maxNumCoins = temp;}
			}
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				if(StashCount.stashArray[i] == maxNumCoins)
					ptm.moveTowardOverboard(i, 3);
			}
			break;
			
		case 21:
			//The player with the most coins must put 3 coins in the stash
			maxNumCoins = 0;
			
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				float temp = StashCount.stashArray[i];
				if(temp > maxNumCoins){maxNumCoins = temp;}
			}
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				if(StashCount.stashArray[i] == maxNumCoins){
					check = checkStash(i, 3);
					StashCount.stashArray[i] -= check;
					StashCount.stashArray[0] += check;
				}
			}
			break;
			
		case 22:
			//Each player recieves 1 coin from the stash
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				StashCount.stashArray[0] -= 1;
				StashCount.stashArray[i] += 1;
			}
			//remove one mutiny card (by choice?)
			if(PlayerTurnMenu.mutinyCard1 != 0 || PlayerTurnMenu.mutinyCard2 != 0
			   || PlayerTurnMenu.mutinyCard3 != 0){
				PlayerTurnMenu.mutinyHelperChoice = false;
				PlayerTurnMenu.wait = true;
				PlayerTurnMenu.showCurrentMutiny = true;
				PlayerTurnMenu.showOptions = false;
				//PlayerTurnMenu.mutinyOption = 0;
			}
			break;
			
		case 23:
			//no effect
			break;
			
		case 24:
			//no effect
			break;
			
		case 25:
			//add 1 coin to the stash for each surviving player
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + i);
				tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
				
				//check each player to see if they are alive
				if(tempStats.getAlive())
					StashCount.stashArray[0] += 1;
				
			}
			break;
			
		case 26:
			//all players in the weapon space must move to the plank
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + i);
				tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
				
				//check to see if player is in weapon place
				//if so, move to start of plank
				int currentSpace = tempStats.getCurrentSpace();
				if(currentSpace == 4){
					ptm.changePlayerLocation(i, 7);
				}
				
			}
			//place this card in the weapon slot
			PlayerTurnMenu.mutinyCard1 = 126;
			
			//add 2 coins to the stash
			StashCount.stashArray[0] += 2;
			break;
			
		case 27:
			//all players in the restraint space space must move to the plank
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + i);
				tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
				
				//check to see if player is in the restraint place
				//if so, move to start of plank
				int currentSpace = tempStats.getCurrentSpace();
				if(currentSpace == 10){
					ptm.changePlayerLocation(i, 7);
				}
				
			}
			//place this card in the restraint slot
			PlayerTurnMenu.mutinyCard2 = 127;
			
			//add 2 coins to the stash
			StashCount.stashArray[0] += 2;
			break;
			
		case 28:
			//all players in the hint space must move to the plank
			for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
				tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + i);
				tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
				
				//check to see if player is in the hint place
				//if so, move to start of plank
				int currentSpace = tempStats.getCurrentSpace();
				if(currentSpace == 13){
					ptm.changePlayerLocation(i, 7);
				}
				
			}
			//place this card in the hint slot
			PlayerTurnMenu.mutinyCard3 = 128;
			
			//add 2 coins to the stash
			StashCount.stashArray[0] += 2;
			break;
			
		default:
			Debug.Log("I think you broke it");
			break;
		}
	}
	
	public int getID(){
		return cardNumber;
	}
	
	public string getTitle(){
		return title;
	}
	
	public string getDesc(){
		return description;		
	}
	
	public string getType(){
		return type;		
	}
	
	public string getTexture(){
		return texture;
	}
	
	public void setID(int id){
		cardNumber = id;
	}
	
	public void setTitle(string t){
		title = t;
	}
	
	public void setDesc(string desc){
		description = desc;
	}
	
	public void setType(string ty){
		if (ty.Equals ("Weapon") || ty.Equals ("Restraint") || ty.Equals ("Hint") || ty.Equals ("Move") || ty.Equals("General") || ty.Equals("Captain"))
			type = ty;
		else 
			type = "";
		
	}
	
	public void setTexture(string tx){
		texture = tx;
	}
	
	//Helper methods for actions
	private void discardCards(string cardType, PlayerTurnMenu ptm){
		for(int i = 1; i <= StartMenu.numberOfPlayers; i++){
			int count = 0;
			tempPlayer = GameObject.Find("/Pirate_Ship/Players/Player_" + i);
			tempStats = tempPlayer.GetComponent("PlayerStats") as PlayerStats;
			
			//check each card
			int [] tempCards = tempStats.getCards();
			for(int j = 0; j < 10; j++){
				//Debug.Log ("Card number: " + j + " with card value " + tempCards[j]);
				if(tempCards[j] != 0){
					if(ptm.cards.getCardType(tempCards[j]).Equals(cardType)){
						tempCards[j] = 0;
						count++;
					}
				}
			}
			tempStats.setCards(tempCards);
			ptm.AddCardsToHand2(count, i);
		}
	}
	
	//returns the amount that is substracted
	private float checkStash(int player, float value){
		if (StashCount.stashArray[player] - value < 0)
			return StashCount.stashArray[player];
		else 
			return value;
	}
}