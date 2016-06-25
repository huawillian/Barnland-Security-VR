using UnityEngine;
using System.Collections;

public class TempHay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine ("destroySoon");
	}

	IEnumerator destroySoon()
	{
		yield return new WaitForSeconds (5.0f);
		Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
