using UnityEngine;
using System.Collections;

// Call event depending on gameobject this is attached to
// Objects:
// Play, Instructions, Leaderboard, Credits
public class Gaze_Controller : MonoBehaviour
{
	public delegate void Gaze();
	public static event Gaze GazePlay;
	public static event Gaze GazeInstructions;
	public static event Gaze GazeLeaderboard;
	public static event Gaze GazeCredits;

	private Coroutine HoverSelectCoroutine = null;
	public float hoverSelectTime;

	public void callEvent()
	{
		switch (this.gameObject.name) {
		case "menu_play":
			GazePlay ();
			break;
		case "menu_instructions":
			GazeInstructions ();
			break;
		case "menu_leader":
			GazeLeaderboard ();
			break;
		case "menu_credits":
			GazeCredits ();
			break;
		default:
			Debug.Log ("Gaze_Controller is used improperly, please check, attached to gameobject: " + this.gameObject.name);
			break;	
		}
	}

	void OnDisable()
	{
		this.StopAllCoroutines ();
	}

	public void OnGazeEnter()
	{
		if (HoverSelectCoroutine != null) {
			StopCoroutine (HoverSelectCoroutine);
		}

		HoverSelectCoroutine = StartCoroutine ("OnHoverSelectCoroutine");
	}

	private IEnumerator OnHoverSelectCoroutine()
	{
		yield return new WaitForSeconds (hoverSelectTime);
		this.callEvent ();
		HoverSelectCoroutine = null;
	}

	public void OnGazeExit()
	{
		if (HoverSelectCoroutine != null) {
			StopCoroutine (HoverSelectCoroutine);
		}

		HoverSelectCoroutine = null;
	}


}
