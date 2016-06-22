using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class gazetest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter()
	{
		Debug.Log( "OnPointerEnter called." );
	}

}
