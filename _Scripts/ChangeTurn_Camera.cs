using UnityEngine;
using System.Collections;

public class ChangeTurn_Camera : MonoBehaviour {
	private Vector3 rotateTo;
	public float rotationSpeed = 45;
	private Vector3 forward;
	public static bool inGame;


	
	void Start () 
	{
		rotateTo = new Vector3(0,35,0);
		forward = new Vector3(-1,0,0);
		PlayerTurnMenu.showOptions = false;
		inGame = false;
	}

	void FixedUpdate () 
	{
		if(inGame == true)
		{
			if(Camera.main.transform.rotation.eulerAngles.y >= 305f)
			{
				if(35 > Camera.main.transform.rotation.eulerAngles.y - 360 && rotateTo.y == 35)
				{
					Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
				}
				else
				{
					PlayerTurnMenu.showOptions = true;
				}
			}
			else if(rotateTo.y > Camera.main.transform.rotation.eulerAngles.y)
			{
				Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
			}
			else
			{
				PlayerTurnMenu.showOptions = true;
			}
		}

	}

	void Update()
	{
	}

	public void Rotate90() 
	{
		if(rotateTo.y + 90f >= 360f)
		{
			rotateTo = new Vector3(0,35,0);
		}
		else
		{
			rotateTo = rotateTo + new Vector3(0,90,0);
		}
	}

	public void MoveSpace(bool moveForward)
	{
		switch (PlayerTurnMenu.playerTurn)
		{
		case 1:
			forward = new Vector3(-1,0,0);
			break;

		case 2:
			forward = new Vector3(0,0,1);
			break;

		case 3:
			forward = new Vector3(1,0,0);
			break;

		case 4:
			forward = new Vector3(0,0,-1);
			break;

		default:
			break;
		}
		if (moveForward == false)
		{forward = -forward;}
		GameObject player = GameObject.Find("Player " + PlayerTurnMenu.playerTurn);

		player.transform.position = player.transform.position - forward;
	}



}
