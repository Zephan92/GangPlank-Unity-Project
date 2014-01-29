using UnityEngine;
using System.Collections;

public class PlayerTurnMenu : MonoBehaviour {
	public static bool showOptions;
	public static int playerTurn;
	private ChangeTurn_Camera ctc;

	// Use this for initialization
	void Start () {
		ctc = gameObject.GetComponent<ChangeTurn_Camera>() as ChangeTurn_Camera;
		playerTurn = 1;
		showOptions = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (showOptions == true)
		{
			GUI.Box(new Rect(10,10,200,120), "Player " + playerTurn + " Options");
			
			if(GUI.Button(new Rect(20,35,180,25), "Skip Turn")) 
			{
				Debug.Log("Skip Turn");
				ctc.Rotate90();
				showOptions = false;
				if (playerTurn == 4)
				{
					playerTurn = 1;
				}
				else
				{
					playerTurn++;
				}
			}
			
			if(GUI.Button(new Rect(20,65,180,25), "Move forward")) 
			{
				Debug.Log("Move");
				ctc.MoveSpace(true);
				ctc.Rotate90();
				showOptions = false;
				if (playerTurn == 4)
				{
					playerTurn = 1;
				}
				else
				{
					playerTurn++;
				}
			}
			
			if(GUI.Button(new Rect(20,95,180,25), "Move backward")) 
			{
				Debug.Log("Move");
				ctc.MoveSpace(false);
				ctc.Rotate90();
				showOptions = false;
				if (playerTurn == 4)
				{
					playerTurn = 1;
				}
				else
				{
					playerTurn++;
				}
			}
		}
		
	}
}
