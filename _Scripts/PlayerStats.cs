using UnityEngine;
using System.Collections;

/*
GangPlank PlayerStats - Updated: 2/25/14

This class is used to keep track of any player non-static variables.
This class also has getters and setter for each variable.
*/

public class PlayerStats : MonoBehaviour {
	public int playerID;//Player ID, "Player 1"
	//Notes on currentSpace:
	//1 = start
	//4 = weapon
	//7 = start of plank
	//10 = restraint
	//13 = hint
	//14-16 = rest of plank tiles
	private int currentSpace;//Player location on the game board
	private int[] cards = new int[10];//Player's hand of card
	private bool alive;//Whether or not player is alive
	private int cardsToAdd;//How many cards that need to be added to a player's hand
	private bool drawRopes;
	private int plays;
	private int lostCards;

	// Use this for initialization
	void Start() 
	{
		if(playerID > StartMenu.numberOfPlayers)
		{
			alive = false;
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			
			foreach (Renderer r in renderers)
			{
				r.enabled = false;
			}
		}
		else
		{
			alive = true;//All players are alive
			currentSpace = 1;//Start space
			for (int i = 0; i < cards.Length; i++)//Initialize the hand
			{cards[i] = 0;}//Initialize to empty
			cardsToAdd = 3;//How many cards a player starts with
			drawRopes = true;
			lostCards = 0;
			PlayerTurnMenu.playersAlive[playerID - 1] = true;
		}
		positionPlayers();
	}

	private void positionPlayers()
	{

		if(playerID == 1)
		{
			if(StartMenu.numberOfPlayers == 2){
				GameObject.Find("/Pirate_Ship/Players/Player_1").transform.position = new Vector3(-1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_2").transform.position = new Vector3(1,5,-1);
			}
			else if(StartMenu.numberOfPlayers == 3){
				GameObject.Find("/Pirate_Ship/Players/Player_1").transform.position = new Vector3(-1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_2").transform.position = new Vector3(0,5,0);
				GameObject.Find("/Pirate_Ship/Players/Player_3").transform.position = new Vector3(1,5,-1);
			}
			else if(StartMenu.numberOfPlayers == 4){
				GameObject.Find("/Pirate_Ship/Players/Player_1").transform.position = new Vector3(-1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_2").transform.position = new Vector3(1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_3").transform.position = new Vector3(-1,5,-1);
				GameObject.Find("/Pirate_Ship/Players/Player_4").transform.position = new Vector3(1,5,-1);
			}
			else if(StartMenu.numberOfPlayers == 5){
				GameObject.Find("/Pirate_Ship/Players/Player_1").transform.position = new Vector3(-1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_2").transform.position = new Vector3(1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_3").transform.position = new Vector3(0,5,0);
				GameObject.Find("/Pirate_Ship/Players/Player_4").transform.position = new Vector3(-1,5,-1);
				GameObject.Find("/Pirate_Ship/Players/Player_5").transform.position = new Vector3(1,5,-1);
			}
			else if(StartMenu.numberOfPlayers == 6){
				GameObject.Find("/Pirate_Ship/Players/Player_1").transform.position = new Vector3(-1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_2").transform.position = new Vector3(1,5,1);
				GameObject.Find("/Pirate_Ship/Players/Player_3").transform.position = new Vector3(-1,5,0);
				GameObject.Find("/Pirate_Ship/Players/Player_4").transform.position = new Vector3(1,5,0);
				GameObject.Find("/Pirate_Ship/Players/Player_5").transform.position = new Vector3(-1,5,-1);
				GameObject.Find("/Pirate_Ship/Players/Player_6").transform.position = new Vector3(1,5,-1);
			}
		}
	}



	//Getter and Setters for PlayerID
	public int getPlayerID()
	{return playerID;}

	public void setPlayerID(int ID)
	{playerID = ID;}


	//Getter and Setters for currentSpace
	public int getCurrentSpace()
	{return currentSpace;}
	
	public void setCurrentSpace(int cs)
	{currentSpace = cs;}


	//Getter and Setters for a player's hand
	public int[] getCards()
	{return cards;}
	
	public void setCards(int[] c)
	{cards = c;}

	public void setCardValue(int i,int v)
	{cards[i] = v;}



	//Getter and Setters for alive
	public bool getAlive()
	{return alive;}
	
	public void setAlive(bool a)
	{alive = a;}


	//Getter/Setters for cardsToAdd + a function to add cards to the queue
	public int getCardsToAdd()
	{return cardsToAdd;}

	public void setCardsToAdd(int i)
	{cardsToAdd = i;}

	public void addCards(int i)
	{cardsToAdd = cardsToAdd + i;}

	//Getter/Setter for drawing rope cards.
	public bool getDrawRopes()
	{return drawRopes;}

	public void setDrawRopes(bool b)
	{drawRopes = b;}

	//Getter/Setters for plays
	public int getPlays()
	{return plays;}

	public void setPlays(int p)
	{plays = p;}

	public void addPlays(int p)
	{plays = plays + p;}

	public int getLostCards()
	{return lostCards;}
	
	public void setLostCards(int i)
	{lostCards = i;}

}
