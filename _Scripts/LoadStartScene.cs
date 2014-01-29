using UnityEngine;
using System.Collections;

public class LoadStartScene : MonoBehaviour {
	public string startScene;

	void Start () 
	{
		Application.LoadLevel(startScene);
	}

}
