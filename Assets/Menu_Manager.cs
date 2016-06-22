using UnityEngine;
using System.Collections;

public class Menu_Manager : MonoBehaviour
{
	public enum MENU_STATE{MAINMENU, PLAY, INSTRUCTIONS, LEADERBOARD, CREDITS, DISABLED};
	public MENU_STATE state = MENU_STATE.MAINMENU;

	public GameObject menuTitle;
	public GameObject menuPlay;
	public GameObject menuInstructions;
	public GameObject menuCredits;
	public GameObject menu_Leaderboard;

	public GameObject credit1;
	public GameObject leader1;
	public GameObject instruction1;
	public GameObject instruction2;
	public GameObject instruction3;
	public GameObject instruction4;
	public GameObject instruction5;
	public GameObject instruction6;

	public float showDuration = 5.0f;

	public GameObject roundManagerObj;
	private Round_Manager roundManager;
	// Use this for initialization
	void Start () {
		roundManager = roundManagerObj.GetComponent<Round_Manager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Event listeners enable and disable
	// Set Event Listeners on enable
	void OnEnable()
	{
		Gaze_Controller.GazePlay += EventPlayHoverSelected;
		Gaze_Controller.GazeInstructions += EventInstructionsHoverSelected;
		Gaze_Controller.GazeLeaderboard += EventLeaderboardHoverSelected;
		Gaze_Controller.GazeCredits += EventCreditsHoverSelected;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Gaze_Controller.GazePlay -= EventPlayHoverSelected;
		Gaze_Controller.GazeInstructions -= EventInstructionsHoverSelected;
		Gaze_Controller.GazeLeaderboard -= EventLeaderboardHoverSelected;
		Gaze_Controller.GazeCredits -= EventCreditsHoverSelected;
	}

	private void EventPlayHoverSelected()
	{
		Debug.Log ("Selected Play Menu");

		if (state == MENU_STATE.MAINMENU) {
			// Start Round from round manager
			// disable main menu
			disableMainMenu();
			state = MENU_STATE.DISABLED;
			roundManager.startRound (1);
		}
	}

	private void EventInstructionsHoverSelected()
	{
		Debug.Log ("Selected Instructions Menu");

		if (state == MENU_STATE.MAINMENU) {
			StartCoroutine (ShowInstructionsCoroutine());
		}
	}

	private void EventLeaderboardHoverSelected()
	{
		Debug.Log ("Selected Leaderboard Menu");

		if (state == MENU_STATE.MAINMENU) {
			StartCoroutine (ShowLeaderboardCoroutine());
		}
	}

	private void EventCreditsHoverSelected()
	{
		Debug.Log ("Selected Credits Menu");

		if (state == MENU_STATE.MAINMENU) {
			StartCoroutine (ShowCreditsCoroutine());
		}
	}

	private IEnumerator ShowCreditsCoroutine()
	{
		state = MENU_STATE.CREDITS;
		disableMainMenu ();
		credit1.SetActive (true);

		yield return new WaitForSeconds (showDuration);

		state = MENU_STATE.MAINMENU;
		enableMainMenu ();
		credit1.SetActive (false);
	}

	private IEnumerator ShowInstructionsCoroutine()
	{
		state = MENU_STATE.LEADERBOARD;
		disableMainMenu ();

		instruction1.SetActive (true);
		yield return new WaitForSeconds (showDuration);
		instruction1.SetActive (false);
		instruction2.SetActive (true);
		yield return new WaitForSeconds (showDuration);
		instruction2.SetActive (false);
		instruction3.SetActive (true);
		yield return new WaitForSeconds (showDuration);
		instruction3.SetActive (false);
		instruction4.SetActive (true);
		yield return new WaitForSeconds (showDuration);
		instruction4.SetActive (false);
		instruction5.SetActive (true);
		yield return new WaitForSeconds (showDuration);
		instruction5.SetActive (false);
		instruction6.SetActive (true);
		yield return new WaitForSeconds (showDuration);
		instruction6.SetActive (false);

		state = MENU_STATE.MAINMENU;
		enableMainMenu ();
		leader1.SetActive (false);
	}
	private IEnumerator ShowLeaderboardCoroutine()
	{
		state = MENU_STATE.LEADERBOARD;
		disableMainMenu ();
		leader1.SetActive (true);

		yield return new WaitForSeconds (showDuration);

		state = MENU_STATE.MAINMENU;
		enableMainMenu ();
		leader1.SetActive (false);
	}

	private void enableMainMenu()
	{
		menuPlay.SetActive (true);
		menuTitle.SetActive (true);
		menuCredits.SetActive (true);
		menuInstructions.SetActive (true);
		menu_Leaderboard.SetActive (true);
	}

	private void disableMainMenu()
	{
		menuPlay.SetActive (false);
		menuTitle.SetActive (false);
		menuCredits.SetActive (false);
		menuInstructions.SetActive (false);
		menu_Leaderboard.SetActive (false);
	}
}
