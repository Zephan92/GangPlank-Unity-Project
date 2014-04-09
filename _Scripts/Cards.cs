using UnityEngine;
using System.Collections;

public class Cards : MonoBehaviour {
	private PlayerTurnMenu ptm;
	private CardObject[] cards = new CardObject[29]; //28 player cards + empty/default
	private CardObject[] captainCards = new CardObject[30]; //29 captain cards + empty/default

	void Start()
	{
		ptm = gameObject.GetComponent("PlayerTurnMenu") as PlayerTurnMenu;
		//Debug.Log("CardDescription Start");
		createCardList();
		createCaptainList();
	}

	//For player cards
	public void playCard(int cardNumber)
	{
		cards[cardNumber].action(ptm);
	}
	
	public string getCardTitle(int cardNumber)
	{
		return cards[cardNumber].getTitle();
	}

	public string getCardType(int cardNumber)
	{
		return cards[cardNumber].getType();
	}
	
	public string getCardDescription(int cardNumber)
	{
		return cards[cardNumber].getDesc();
	}

	public string getCardTexture(int cardNumber){
		return cards[cardNumber].getTexture();
	}
	
	public void createCardList(){
		int i = 1;

		CardObject c = new CardObject();
		c.setDesc("");
		cards[0] = c;

		CardObject c2 = new CardObject(); //KEEP
		c2.setID(i);
		c2.setTitle("Dagger");
		c2.setDesc("Steal 1 coin " +
			"\nfrom any player");
		c2.setType("Weapon");
		c2.setTexture("Dagger");
		cards [i] = c2;
		i++;

		CardObject c3 = new CardObject(); //KEEP
		c3.setID(i);
		c3.setTitle("Cutlass");
		c3.setDesc("Steal 2 coins " +
			"\nfrom any player");
		c3.setType("Weapon");
		c3.setTexture("Cutlass");
		cards [i] = c3;
		i++;

		CardObject c4 = new CardObject();
		c4.setID(i);
		c4.setTitle("Walk");
		c4.setDesc("Move 1 space.");
		c4.setType("Move");
		c4.setTexture("Walk");
		cards [i] = c4;
		i++;

		CardObject c5 = new CardObject(); //KEEP
		c5.setID(i);
		c5.setTitle("Pistol");
		c5.setDesc("Move an opponent " +
		           "\n1 space in any direction.");
		c5.setType("Weapon");
		c5.setTexture("Pistol");
		cards [i] = c5;
		i++;

		CardObject c6 = new CardObject(); //KEEP
		c6.setID(i);
		c6.setTitle("Musket");
		c6.setDesc("Move an opponent " +
		           "\nup to 2 spaces in any direction.");
		c6.setType("Weapon");
		c6.setTexture("Musket");
		cards [i] = c6;
		i++;

		CardObject c7 = new CardObject(); //KEEP
		c7.setID(i);
		c7.setTitle("Powder Keg");
		c7.setDesc("Move all opponents " +
		           "\n2 spaces towards Overboard.");
		c7.setType("Weapon");
		c7.setTexture("Powder Keg");
		cards [i] = c7;
		i++;

		CardObject c8 = new CardObject(); //KEEP
		c8.setID(i);
		c8.setTitle("Rope");
		c8.setDesc("If you are knocked overboard, discard this card, " +
			"draw a card and place your piece at the end of the plank.");
		c8.setType("Restraint");
		c8.setTexture("Rope");
		cards [i] = c8;
		i++;

		CardObject c9 = new CardObject(); //KEEP
		c9.setID(i);
		c9.setTitle("Ball and Chain");
		c9.setDesc("Choose an opponent." +
			"That player only plays and draws 1 card" +
			"during their next turn.");
		c9.setType("Restraint");
		c9.setTexture("Ball and Chain");
		cards [i] = c9;
		i++;

		CardObject c10 = new CardObject(); //KEEP
		c10.setID(i);
		c10.setTitle("Net");
		c10.setDesc("Pull an opponent " +
		            "\ninto your space");
		c10.setType("Restraint");
		c10.setTexture("Net");
		cards [i] = c10;
		i++;

		CardObject c11 = new CardObject(); //KEEP
		c11.setID(i);
		c11.setTitle("Manacles");
		c11.setDesc("Choose an opponent to lost their next turn.");
		c11.setType("Restraint");
		c11.setTexture("Manacles");
		cards [i] = c11;
		i++;

		CardObject c12 = new CardObject(); //KEEP
		c12.setID(i);
		c12.setTitle("Grappling Hook");
		c12.setDesc("Switch places " +
		            "\nwith an opponent.");
		c12.setType("Restraint");
		c12.setTexture("Grappling Hook");
		cards [i] = c12;
		i++;

		CardObject c13 = new CardObject(); //KEEP
		c13.setID(i);
		c13.setTitle("The Anchor");
		c13.setDesc("Choose an opponent. That player cannot use Rope cards" +
			"for the rest of the game");
		c13.setType("Restraint");
		c13.setTexture("Anchor");
		cards [i] = c13;
		i++;

		CardObject c14 = new CardObject(); //KEEP
		c14.setID(i);
		c14.setTitle("Spyglass");
		c14.setDesc("Draw 2 card and play them immediately.");
		c14.setType("Hint");
		c14.setTexture("Spyglass");
		cards [i] = c14;
		i++;

		CardObject c15 = new CardObject(); //KEEP
		c15.setID(i);
		c15.setTitle("Compass");
		c15.setDesc("Move to any " +
		            "\nMutiny space.");
		c15.setType("Hint");
		c15.setTexture("Compass");
		cards[i] = c15;
		i++;

		CardObject c16 = new CardObject(); //KEEP
		c16.setID(i);
		c16.setTitle("Captain's Map");
		c16.setDesc("Draw 3 cards and then discard 3 cards.");
		c16.setType("Hint");
		c16.setTexture("Map");
		cards [i] = c16;
		i++;

		CardObject c17 = new CardObject(); //KEEP
		c17.setID(i);
		c17.setTitle("Captain's Key");
		c17.setDesc("Take 1 coin from the stash.");
		c17.setType("Hint");
		c17.setTexture("Key");
		cards [i] = c17;
		i++;

		CardObject c18 = new CardObject(); //KEEP
		c18.setID(i);
		c18.setTitle("Dash");
		c18.setDesc("Move up to 3 spaces");
		c18.setType("Move");
		c18.setTexture("Dash");
		cards [i] = c18;
		i++;

		CardObject c19 = new CardObject(); //KEEP
		c19.setID(i);
		c19.setTitle("Run");
		c19.setDesc("Move up to 5 spaces");
		c19.setType("Move");
		c19.setTexture("Run");
		cards [i] = c19;
		i++;

		CardObject c20 = new CardObject(); //KEEP
		c20.setID(i);
		c20.setTitle("Bull Rush");
		c20.setDesc("Move 1 space.\n" +
			"Take 1 coin from each opponent in that space.");
		c20.setType("Move");
		c20.setTexture("Bull Rush");
		cards [i] = c20;
		i++;

		CardObject c21 = new CardObject();
		c21.setID(i);
		c21.setTitle("Hook");
		c21.setDesc("Move an opponent 1 space and steal 1 coin from that player.");
		c21.setType("Weapon");
		c21.setTexture("Hook");
		cards [i] = c21;
		i++;

		CardObject c22 = new CardObject();
		c22.setID(i);
		c22.setTitle("Treasure Chest");
		c22.setDesc("Take 2 coins from the stash.");
		c22.setType("Hint");
		c22.setTexture("Treasure Chest");
		cards [i] = c22;
		i++;

		CardObject c23 = new CardObject(); //KEEP
		c23.setID(i);
		c23.setTitle("Pickpocket");
		c23.setDesc("Move up to 3 spaces " +
		            "\nand take up to 1 coin " +
		            "\nfrom an opponent in one of those spaces.");
		c23.setType("Move");
		c23.setTexture("Pickpocket");
		cards [i] = c23;
		i++;

		CardObject c24 = new CardObject();
		c24.setID(i);
		c24.setTitle("Cannon");
		c24.setDesc("Steal 1 coin from each player.");
		c24.setType("Weapon");
		c24.setTexture("Cannon");
		cards [i] = c24;
		i++;

		CardObject c25 = new CardObject(); //NEW
		c25.setID(i);
		c25.setTitle("Black Cat");
		c25.setDesc("Move 1 space.\n" +
			"Draw 1 card and play immediately.");
		c25.setType("Move");
		c25.setTexture("Black Cat");
		cards [i] = c25;
		i++;
		
		CardObject c26 = new CardObject(); //NEW
		c26.setID(i);
		c26.setTitle("Chatty Parrot");
		c26.setDesc("Take 2 coins from the stash.\n" +
			"Draw a second Captain's Card this turn.");
		c26.setType("Hint");
		c26.setTexture("Chatty Parrot");
		cards [i] = c26;
		i++;

		CardObject c27 = new CardObject(); //NEW
		c27.setID(i);
		c27.setTitle("Captain's Clock");
		c27.setDesc("Take 5 coins from the stash.\n" +
			"Give 1 to each opponent.");
		c27.setType("Move");
		c27.setTexture("Clock");
		cards [i] = c27;
		i++;

		CardObject c28 = new CardObject(); //NEW
		c28.setID(i);
		c28.setTitle("Lucky Coin");
		c28.setDesc("Do not draw a Captain's Card this turn.");
		c28.setType("Hint");
		c28.setTexture("Coin");
		cards [i] = c28;
		i++;
	}

	//For captain cards
	public void playCaptainCard(int cardNumber)
	{
		captainCards[cardNumber].actionCpt(ptm);
	}
	
	public string getCCardTitle(int cardNumber)
	{
		return captainCards[cardNumber].getTitle();
	}
	
	public string getCCardType(int cardNumber)
	{
		return captainCards[cardNumber].getType();
	}
	
	public string getCCardDescription(int cardNumber)
	{
		return captainCards[cardNumber].getDesc();
	}

	public string getCCardTexture(int cardNumber){
		return captainCards[cardNumber].getTexture();
	}
	
	public void createCaptainList(){
		int i = 1;
	
		//default
		CardObject c = new CardObject();
		c.setDesc("");
		captainCards[0] = c;

		CardObject c2 = new CardObject();
		c2.setID(i);
		c2.setTitle("The Captain Orders You to Man Your Post");
		c2.setDesc("Each player must discard any Move cards in their hands and draw that many cards.");
		c2.setType("Captain");
		c2.setTexture("8");
		captainCards [i] = c2;
		i++;

		CardObject c3 = new CardObject();
		c3.setID(i);
		c3.setTitle("The Captain Gives Harsh Orders");
		c3.setDesc("All players in a Mutiny space must move to the Plank.\n Place this card in any Mutiny slot.\n Add 2 coins to the stash.");
		c3.setType("Captain");
		c3.setTexture("1");
		captainCards [i] = c3;
		i++;

		CardObject c4 = new CardObject();
		c4.setID(i);
		c4.setTitle("The Captain Teaches You About Karma");
		c4.setDesc("The player with the most coins must give 1 coin to the player with the fewest coins.");
		c4.setType("Captain");
		c4.setTexture("9");
		captainCards [i] = c4;
		i++;
		
		CardObject c5 = new CardObject();
		c5.setID(i);
		c5.setTitle("The Captain Doesn't Like Your Backtalk");
		c5.setDesc("The current player must move 2 spaces towards overboard.");
		c5.setType("Captain");
		c5.setTexture("10");
		captainCards [i] = c5;
		i++;
		
		CardObject c6 = new CardObject();
		c6.setID(i);
		c6.setTitle("The Captain Orders You to Replace a Sail");
		c6.setDesc("The current player must put 2 coins in the stash.");
		c6.setType("Captain");
		c6.setTexture("11");
		captainCards [i] = c6;
		i++;
		
		CardObject c7 = new CardObject();
		c7.setID(i);
		c7.setTitle("The Captain is Angry Today");
		c7.setDesc("The current player must draw 2 more Captain's Cards this turn.");
		c7.setType("Captain");
		c7.setTexture("12");
		captainCards [i] = c7;
		i++;
		
		CardObject c8 = new CardObject();
		c8.setID(i);
		c8.setTitle("The Captain Orders You to Fetch His Grog");
		c8.setDesc("The current player must move to the Start space.");
		c8.setType("Captain");
		c8.setTexture("13");
		captainCards [i] = c8;
		i++;
		
		CardObject c9 = new CardObject();
		c9.setID(i);
		c9.setTitle("The Captain Orders You to Port");
		c9.setDesc("Each player may discard any number of cards to draw that many cards.");
		c9.setType("Captain");
		c9.setTexture("14");
		captainCards [i] = c9;
		i++;
		
		CardObject c10 = new CardObject();
		c10.setID(i);
		c10.setTitle("The Captain Uses You as a Footstool");
		c10.setDesc("The current player must move 4 spaces towards Overboard.");
		c10.setType("Captain");
		c10.setTexture("15");
		captainCards [i] = c10;
		i++;
		
		CardObject c11 = new CardObject();
		c11.setID(i);
		c11.setTitle("The Captain Buys Fake Jewels");
		c11.setDesc("Remove 5 coins from the stash.");
		c11.setType("Captain");
		c11.setTexture("16");
		captainCards [i] = c11;
		i++;
		
		CardObject c12 = new CardObject();
		c12.setID(i);
		c12.setTitle("The Captain has Expensive Taste");
		c12.setDesc("Remove 3 coins from the stash.");
		c12.setType("Captain");
		c12.setTexture("17");
		captainCards [i] = c12;
		i++;
		
		CardObject c13 = new CardObject();
		c13.setID(i);
		c13.setTitle("The Captain Hears Rumors of a Mutiny");
		c13.setDesc("Move each player 2 spaces toward Overboard.");
		c13.setType("Captain");
		c13.setTexture("19");
		captainCards [i] = c13;
		i++;
		
		CardObject c14 = new CardObject();
		c14.setID(i);
		c14.setTitle("The Captain Gambles");
		c14.setDesc("Remove 2 coins from the stash");
		c14.setType("Captain");
		c14.setTexture("18");
		captainCards [i] = c14;
		i++;
		
		CardObject c15 = new CardObject();
		c15.setID(i);
		c15.setTitle("The Captain Knows about the Mutiny");
		c15.setDesc("Move each player 4 spaces towards Overboard.");
		c15.setType("Captain");
		c15.setTexture("20");
		captainCards[i] = c15;
		i++;
		
		CardObject c16 = new CardObject();
		c16.setID(i);
		c16.setTitle("The Captain gets Seasick");
		c16.setDesc("Move each player 1 space towards Overboard.");
		c16.setType("Captain");
		c16.setTexture("21");
		captainCards [i] = c16;
		i++;

		CardObject c17 = new CardObject();
		c17.setID(i);
		c17.setTitle("The Captain is a Cheater");
		c17.setDesc("Each player must put 1 coin into the stash.");
		c17.setType("Captain");
		c17.setTexture("22");
		captainCards [i] = c17;
		i++;

		CardObject c18 = new CardObject();
		c18.setID(i);
		c18.setTitle("The Captain Wants the Rigging Fixed");
		c18.setDesc("Each player must discard any Restraint cards in their hand and draw that many cards.");
		c18.setType("Captain");
		c18.setTexture("5");
		captainCards [i] = c18;
		i++;

		CardObject c19 = new CardObject();
		c19.setID(i);
		c19.setTitle("The Captain Wants You to Fight the Navy");
		c19.setDesc("Each player must discard any Weapon cards in their hand and draw that many cards.");
		c19.setType("Captain");
		c19.setTexture("6");
		captainCards [i] = c19;
		i++;

		CardObject c20 = new CardObject();
		c20.setID(i);
		c20.setTitle("The Captain Wants the Treasure Moved");
		c20.setDesc("Each player must discard any Hint cards in their hand and draw that many cards.");
		c20.setType("Captain");
		c20.setTexture("7");
		captainCards [i] = c20;
		i++;

		CardObject c21 = new CardObject();
		c21.setID(i);
		c21.setTitle("The Captain Orders You to Walk the Plank");
		c21.setDesc("The player with the most coins must move 3 spaces towards Overboard.");
		c21.setType("Captain");
		c21.setTexture("23");
		captainCards [i] = c21;
		i++;

		CardObject c22 = new CardObject();
		c22.setID(i);
		c22.setTitle("The Captain Orders to Share the Loot");
		c22.setDesc("The player with the most coins must put 3 coins in the stash.");
		c22.setType("Captain");
		c22.setTexture("24");
		captainCards [i] = c22;
		i++;

		CardObject c23 = new CardObject();
		c23.setID(i);
		c23.setTitle("The Captain Tries to Bribe the Crew");
		c23.setDesc("Each player receives 1 coin from the stash.\nRemove 1 Mutiny card.");
		c23.setType("Captain");
		c23.setTexture("25");
		captainCards [i] = c23;
		i++;

		CardObject c24 = new CardObject();
		c24.setID(i);
		c24.setTitle("The Captain is Sleeping");
		c24.setDesc("Nothing happens.");
		c24.setType("Captain");
		c24.setTexture("26");
		captainCards [i] = c24;
		i++;

		CardObject c25 = new CardObject();
		c25.setID(i);
		c25.setTitle("The Captain is Happy with the Weather");
		c25.setDesc("Nothing happens.");
		c25.setType("Captain");
		c25.setTexture("27");
		captainCards [i] = c25;
		i++;

		CardObject c26 = new CardObject();
		c26.setID(i);
		c26.setTitle("The Captain Spots a Merchant Ship");
		c26.setDesc("Add 1 coin to the stash for each surviving player.");
		c26.setType("Captain");
		c26.setTexture("28");
		captainCards [i] = c26;
		i++;

		CardObject c27 = new CardObject();
		c27.setID(i);
		c27.setTitle("The Captain Gives Harsh Orders");
		c27.setDesc("Players in the Weapon space must move to the Plank. " +
			"Place this card in a Weapon slot." +
			"Add 2 coins to the stash.");
		c27.setType("Captain");
		c27.setTexture("2");
		captainCards [i] = c27;
		i++;

		CardObject c28 = new CardObject();
		c28.setID(i);
		c28.setTitle("The Captain Gives Harsh Orders");
		c28.setDesc("Players in the Restraint space must move to the Plank. " +
		            "Place this card in a Restraint slot." +
		            "Add 2 coins to the stash.");
		c28.setType("Captain");
		c28.setTexture("3");
		captainCards [i] = c28;
		i++;

		CardObject c29 = new CardObject();
		c29.setID(i);
		c29.setTitle("The Captain Gives Harsh Orders");
		c29.setDesc("Players in the Hint space must move to the Plank. " +
		            "Place this card in a Hint slot." +
		            "Add 2 coins to the stash.");
		c29.setType("Captain");
		c29.setTexture("4");
		captainCards [i] = c29;
		i++;
	}
}
