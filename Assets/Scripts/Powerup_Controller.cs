using UnityEngine;
using System.Collections;

public class Powerup_Controller : MonoBehaviour
{
	//type of powerup
	public enum POWERUP_TYPE{BOMB, TRACTOR, HAY};
	public POWERUP_TYPE type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.Rotate (Vector3.up * Time.deltaTime * 60);
	}
}
