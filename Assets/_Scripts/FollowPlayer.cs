using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	private Vector3 cameraPosition;
	private Vector3 playerPosition;
	private GameObject go; 
	// Use this for initialization
	void Start () {
	
	}


	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateCamera(int player)
	{
		go = GameObject.Find("/Pirate_Ship/Players/Player_" + player) as GameObject;
		if(go != null)
		{
			playerPosition = go.transform.position;
			cameraPosition = playerPosition + new Vector3(0,15,-15);
			this.gameObject.transform.position = cameraPosition;
			//this.gameObject.transform.rotation.eulerAngles = new Vector3(); Fix this later

		}
	}
}
