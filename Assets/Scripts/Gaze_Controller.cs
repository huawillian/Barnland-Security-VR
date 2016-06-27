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
	public static event Gaze PlaySelectSound;

	public delegate void GazeInput(string keyboard);
	public static event GazeInput GazeInputKeyboard;

	private Coroutine HoverSelectCoroutine = null;
	public float hoverSelectTime;

	public void callGazePlay()
	{
		GazePlay ();
	}

	public void callGazeInstructions()
	{
		GazeInstructions ();
	}

	public void callLeaderboard()
	{
		GazeLeaderboard ();
	}

	public void callCredits()
	{
		GazeCredits ();
	}

	public void callPlaySound()
	{
		PlaySelectSound ();
	}

	public void callGazeInputKeyboard(string c)
	{
		GazeInputKeyboard (c);
	}

	public void callEvent()
	{
		PlaySelectSound ();

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
			GazeInputKeyboard (this.gameObject.name);
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
