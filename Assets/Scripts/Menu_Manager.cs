using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	private Firebase_Leaderboard_Controller firebaseController;

	Firebase_Leaderboard_Controller.LeaderboardEntry[] leaderboardData = null;
	public string notFirebaseLeaderboardText = "Unable to retrieve leaderboard. \nPlease check internet connection.";
	public bool isFirebase = false;

	// Use this for initialization
	void Start () {
		roundManager = roundManagerObj.GetComponent<Round_Manager> ();
		firebaseController = roundManagerObj.GetComponent<Firebase_Leaderboard_Controller> ();
		StartCoroutine ("setLeaderboardCoroutine");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator setLeaderboardCoroutine()
	{
		yield return new WaitForSeconds (2.0f);
		firebaseController.retrieveLeaderboardOnFirebase ();
	}

	// Event listeners enable and disable
	// Set Event Listeners on enable
	void OnEnable()
	{
		Gaze_Controller.GazePlay += EventPlayHoverSelected;
		Gaze_Controller.GazeInstructions += EventInstructionsHoverSelected;
		Gaze_Controller.GazeLeaderboard += EventLeaderboardHoverSelected;
		Gaze_Controller.GazeCredits += EventCreditsHoverSelected;
		Firebase_Leaderboard_Controller.OnDataReceived += this.OnEventDataReceived;
		Firebase_Leaderboard_Controller.OnDataTimeout += this.OnEventDataTimeOut;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Gaze_Controller.GazePlay -= EventPlayHoverSelected;
		Gaze_Controller.GazeInstructions -= EventInstructionsHoverSelected;
		Gaze_Controller.GazeLeaderboard -= EventLeaderboardHoverSelected;
		Gaze_Controller.GazeCredits -= EventCreditsHoverSelected;
		Firebase_Leaderboard_Controller.OnDataReceived -= this.OnEventDataReceived;
		Firebase_Leaderboard_Controller.OnDataTimeout -= this.OnEventDataTimeOut;
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

		string textToSet;

		if (isFirebase) {
			textToSet = "    Leaderboard\n\n";

			for (int i = 1; i <= 10; i++) {
				textToSet += i + ": " + leaderboardData [i - 1].name + " | " + leaderboardData [i - 1].score + "\n";
			}
		} else {
			textToSet = notFirebaseLeaderboardText;
		}

		leader1.GetComponent<TextMesh> ().text = textToSet;

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

	void OnEventDataReceived(Firebase_Leaderboard_Controller.LeaderboardEntry[] data)
	{
		isFirebase = true;
		leaderboardData = (Firebase_Leaderboard_Controller.LeaderboardEntry[]) data.Clone ();
	}

	void OnEventDataTimeOut(Firebase_Leaderboard_Controller.LeaderboardEntry[] data)
	{
		isFirebase = false;
	}

}
