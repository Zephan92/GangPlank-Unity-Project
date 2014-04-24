using System.Linq;
using System;
using System.IO;

public class Card
{
    public string card_type;
	public string card_id;
	public int card_num;
    public Card(string type, int id)
    {
        card_type = type;
        card_id = ""+id;
    }
    public void setType (string newType)
	{
		card_type = newType;
	}
	public void setNum (int newNum)
	{
		card_num = newNum;
	}
	public void setId (string newId)
	{
		card_id = newId;
	}
	public string type_of_card ( string aId)
	{
		//if ( StrComp( aId, card_id) == 0)
		if ( aId.Equals(card_id) )
			return card_type;
		else
			return null;
	}
	public int num_of_card ( string aId)
	{
		if ( aId.Equals(card_id) )
			return card_num;
		else
			return 0;
	}
}

class Program
{
    static void Main()
    {	
		int counter = 0;
		string[] line= new string[144];
        Card[] card_database = new Card[144];
        // Read the file to the string array.
		System.IO.StreamReader file = new System.IO.StreamReader("database.txt");
		while((line[counter] = file.ReadLine()) != null)
		{
			card_database[counter] = new Card(null, 0);
			card_database[counter].setId (line[counter]);
			switch (counter)
			{
				case 0:
				case 1:
					card_database[counter].setType ("treasure_chest");
					card_database[counter].setNum (1);
					break;
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
					card_database[counter].setType ("rope");
					card_database[counter].setNum (2);
					break;
				case 8:
				case 9:
					card_database[counter].setType ("compass");
					card_database[counter].setNum (3);
					break;
				case 10:
				case 11:
					card_database[counter].setType ("lucky_coin");
					card_database[counter].setNum (4);
					break;
				case 12:
				case 13:
					card_database[counter].setType ("spy_glass");
					card_database[counter].setNum (5);
					break;
				case 14:
				case 15:
					card_database[counter].setType (" captain_map");
					card_database[counter].setNum (6);
					break;
				case 16:
				case 17:
					card_database[counter].setType ("chatty_parrot");
					card_database[counter].setNum (7);
					break;
				case 18:
				case 19:
					card_database[counter].setType ("anchor");
					card_database[counter].setNum (8);
					break;
				case 20:
				case 21:
				case 22:
					card_database[counter].setType ("net");
					card_database[counter].setNum (9);
					break;
				case 23:
				case 24:
					card_database[counter].setType ("ball_chain");
					card_database[counter].setNum (10);
					break;
				case 25:
				case 26:
					card_database[counter].setType ("manacles");
					card_database[counter].setNum (11);
					break;
				case 27:
				case 28:
				case 29:
					card_database[counter].setType ("grappling_hook");
					card_database[counter].setNum (12);
					break;
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
					card_database[counter].setType ("run");
					card_database[counter].setNum (13);
					break;
				case 35:
				case 36:
				case 37:
				case 38:
					card_database[counter].setType ("walk");
					card_database[counter].setNum (14);
					break;
				case 39:
				case 40:
				case 41:
					card_database[counter].setType ("ball_rush");
					card_database[counter].setNum (15);
					break;
				case 42:
				case 43:
				case 44:
					card_database[counter].setType ("pickpocket");
					card_database[counter].setNum (16);
					break;
				case 45:
				case 46:
				case 47:
				case 48:
				case 49:
				case 50:
				case 51:
					card_database[counter].setType ("dash");
					card_database[counter].setNum (17);
					break;
				case 52:
				case 53:
					card_database[counter].setType ("captain_clock");
					card_database[counter].setNum (18);
					break;
				case 54:
				case 55:
				case 56:
					card_database[counter].setType ("black_cat");
					card_database[counter].setNum (19);
					break;
				case 57:
				case 58:
					card_database[counter].setType ("cannon");
					card_database[counter].setNum (20);
					break;
				case 59:
				case 60:
					card_database[counter].setType ("power_keg");
					card_database[counter].setNum (21);
					break;
				case 61:
				case 62:
				case 63:
				case 64:
					card_database[counter].setType ("dagger");
					card_database[counter].setNum (22);
					break;
				case 65:
				case 66:
				case 67:
				case 68:
					card_database[counter].setType ("pistol");
					card_database[counter].setNum (23);
					break;
				case 69:
				case 70:
				case 71:
					card_database[counter].setType ("cutlass");
					card_database[counter].setNum (24);
					break;
				case 72:
				case 73:
				case 74:
					card_database[counter].setType ("captain_key");
					card_database[counter].setNum (25);
					break;
				case 75:
				case 76:
				case 77:
					card_database[counter].setType ("musket");
					card_database[counter].setNum (26);
					break;
				case 78:
				case 79:
					card_database[counter].setType ("hook");
					card_database[counter].setNum (27);
					break;
				case 80:
					card_database[counter].setType ("harsh_order_mutiny");
					card_database[counter].setNum (29);
					break;
				case 81:
					card_database[counter].setType ("harsh_order_weapon");
					card_database[counter].setNum (30);
					break;
				case 82:
				case 83:
					card_database[counter].setType ("harsh_order_restraint");
					card_database[counter].setNum (31);
					break;
				case 84:
				case 85:
					card_database[counter].setType ("harsh_order_hint");
					card_database[counter].setNum (32);
					break;
				case 86:
				case 87:
					card_database[counter].setType ("rigging_fixed");
					card_database[counter].setNum (33);
					break;
				case 88:
				case 89:
					card_database[counter].setType ("fight_navy");
					card_database[counter].setNum (34);
					break;
				case 90:
					card_database[counter].setType ("treasure_moved");
					card_database[counter].setNum (35);
					break;
				case 91:
					card_database[counter].setType ("man_post");
					card_database[counter].setNum (36);
					break;
				case 92:
				case 93:
					card_database[counter].setType ("karma");
					card_database[counter].setNum (37);
					break;
				case 94:
				case 95:
				case 96:
					card_database[counter].setType ("backtalk");
					card_database[counter].setNum (38);
					break;
				case 97:
				case 98:
				case 99:
					card_database[counter].setType ("sail");
					card_database[counter].setNum (39);
					break;
				case 100:
				case 101:
					card_database[counter].setType ("angry");
					card_database[counter].setNum (40);
					break;
				case 102:
					card_database[counter].setType ("fetch_grog");
					card_database[counter].setNum (41);
					break;
				case 103:
				case 104:
					card_database[counter].setType ("port");
					card_database[counter].setNum (42);
					break;
				case 105:
					card_database[counter].setType ("footstool");
					card_database[counter].setNum (43);
					break;	
				case 106:
					card_database[counter].setType ("fake_jewels");
					card_database[counter].setNum (44);
					break;
				case 107:		
				case 108:
					card_database[counter].setType ("expensive_taste");
					card_database[counter].setNum (45);
					break;	
				case 109:		
				case 110:
				case 111:
					card_database[counter].setType ("gamble");
					card_database[counter].setNum (46);
					break;
				case 112:
				case 113:
					card_database[counter].setType ("rumors_mutiny");
					card_database[counter].setNum (47);
					break;
				case 114:
					card_database[counter].setType ("know_mutiny");
					card_database[counter].setNum (48);
					break;
				case 115:		
				case 116:
				case 117:
					card_database[counter].setType ("seasick");
					card_database[counter].setNum (49);
					break;
				case 118:
				case 119:
					card_database[counter].setType ("cheater");
					card_database[counter].setNum (50);
					break;
				case 120:
				case 121:
				case 122:
					card_database[counter].setType ("walk_plank");
					card_database[counter].setNum (51);
					break;
				case 123:
				case 124:
				case 125:
					card_database[counter].setType ("share_loot");
					card_database[counter].setNum (52);
					break;
				case 126:
					card_database[counter].setType ("bribe_crew");
					card_database[counter].setNum (53);
					break;
				case 127:
				case 128:
				case 129:
					card_database[counter].setType ("sleeping");
					card_database[counter].setNum (54);
					break;
				case 130:
				case 131:
				case 132:
				case 133:
				case 134:
				case 135:
				case 136:
				case 137:
				case 138:
				case 139:
				case 140:
				case 141:
				case 142:
					card_database[counter].setType ("happy_weather");
					card_database[counter].setNum (55);
					break;
				case 143:
					card_database[counter].setType ("merchant_ship");
					card_database[counter].setNum (56);
					break;
				default:
					break;
			}
			counter++;
		}
		file.Close();

		File.WriteAllLines("file.txt", line.Select((str, i)=> str+", "+(card_database[i]==null?"null":(card_database[i].card_num+", "+card_database[i].card_type))).ToArray());
		
    }
}


