using UnityEngine;
using System.Collections;

public class MakeStaticObject : MonoBehaviour {
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
