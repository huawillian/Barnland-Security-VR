using UnityEngine;
using System.Collections;

public class Gaze_Controller_NOVR : MonoBehaviour {
	
	private Coroutine HoverSelectCoroutine = null;
	public float hoverSelectTime = 1.0f;
	public string gazeName = "";

	Gaze_Controller gazeController;

	// Use this for initialization
	void Start () {
		this.gazeController = this.GetComponent<Gaze_Controller> ();
	}

	private IEnumerator OnHoverSelectCoroutine()
	{
		yield return new WaitForSeconds (hoverSelectTime);
		this.callEvent ();
		HoverSelectCoroutine = null;
	}

	public void callEvent()
	{
		gazeController.callPlaySound ();

		switch (gazeName) {
		case "menu_play":
			gazeController.callGazePlay ();
			break;
		case "menu_instructions":
			gazeController.callGazeInstructions ();
			break;
		case "menu_leader":
			gazeController.callLeaderboard ();
			break;
		case "menu_credits":
			gazeController.callCredits ();
			break;
		default:
			gazeController.callGazeInputKeyboard (gazeName);
			break;	
		}
	}

	public void OnGazeExit()
	{
		if (HoverSelectCoroutine != null) {
			StopCoroutine (HoverSelectCoroutine);
		}

		HoverSelectCoroutine = null;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Gaze")
			return;

		Debug.Log (other.name);

		if (HoverSelectCoroutine != null) {
			StopCoroutine (HoverSelectCoroutine);
		}

		gazeName = other.name;

		HoverSelectCoroutine = StartCoroutine ("OnHoverSelectCoroutine");
	}
		
	void OnTriggerExit(Collider other)
	{
		if (other.tag != "Gaze")
			return;

		if (HoverSelectCoroutine != null) {
			StopCoroutine (HoverSelectCoroutine);
		}

		HoverSelectCoroutine = null;
	}




}
