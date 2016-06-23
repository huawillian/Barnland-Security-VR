using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Leaderboard_Manager : MonoBehaviour
{
	Firebase_Leaderboard_Controller.LeaderboardEntry[] leaderboard = new Firebase_Leaderboard_Controller.LeaderboardEntry[10];
	Firebase_Leaderboard_Controller firebaseController;

	public bool isFirebase = false;

	public GameObject guiObj;
	private GUI_Controller guicontroller;

	public GameObject canvasObj;
	public GameObject scoreObj;
	public GameObject returnToMainMenuObj;
	public GameObject unavailableObj;
	public GameObject leaderboardTitleObj;
	public GameObject leaderboardObj;
	public GameObject nameInputObj;
	public GameObject leaderboardName;

	public string leaderboardNameString = "Insert Name";
	public bool isInputName = false;
	public int score = 0;

	public GameObject headObj;

	// Use this for initialization
	void Start () {
		firebaseController = this.GetComponent<Firebase_Leaderboard_Controller> ();
		guicontroller = guiObj.GetComponent<GUI_Controller> ();
		hideLeaderboard ();
	}

	private void OnKeyboardGaze(string input)
	{
		if (input == "Return Main Menu") {
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
			
		if (input == "Enter Name") {
			isInputName = true;
			AddEntry (leaderboardNameString, score);
		}

		if (input == "Back") {
			if (leaderboardNameString == "Insert Name")
				leaderboardNameString = "";

			if (leaderboardNameString.Length > 0) {
				leaderboardNameString = leaderboardNameString.Substring (0, leaderboardNameString.Length - 1);
				leaderboardName.GetComponent<TextMesh> ().text = leaderboardNameString;
			}
		}

		if (input.Length == 1) {
			if (leaderboardNameString == "Insert Name")
				leaderboardNameString = "";
			
			leaderboardNameString += input;
			leaderboardName.GetComponent<TextMesh> ().text = leaderboardNameString;
		}
	}

	public void hideLeaderboard()
	{
		canvasObj.SetActive (false);
		scoreObj.SetActive (false);
		returnToMainMenuObj.SetActive (false);
		unavailableObj.SetActive (false);
		leaderboardTitleObj.SetActive (false);
		leaderboardObj.SetActive (false);
		nameInputObj.SetActive (false);
	}

	public void showLeaderboard()
	{
		canvasObj.SetActive (true);
		canvasObj.transform.localRotation = Quaternion.Euler (new Vector3(0, headObj.transform.localRotation.eulerAngles.y, 0));
	}

	public void AddEntry(string tempName, int tempScore)
	{
		if (!isFirebase)
			return;

		bool insert = false;
		string previousName = "";
		int previousScore = 0;
		string currName = "";
		int currScore = 0;

		if (isEntryHighScore (tempScore)) {
			for (int i = 0; i < 10; i++) {
				if (!insert && tempScore > leaderboard [i].score) {
					insert = true;
					previousName = leaderboard [i].name;
					previousScore = leaderboard [i].score;
					leaderboard [i].name = tempName;
					leaderboard [i].score = tempScore;
				} else if (insert) {
					currName = leaderboard [i].name;
					currScore = leaderboard [i].score;
					leaderboard [i].name = previousName;
					leaderboard [i].score = previousScore;
					previousName = currName;
					previousScore = currScore;
				}
			}

			firebaseController.leaderboardEntriesToFirebase (leaderboard);
		}
	}

	public IEnumerator StartEndGameLeaderboard(int tempScore)
	{
		firebaseController.retrieveLeaderboardOnFirebase ();
		guicontroller.hideCanvas ();
		score = tempScore;
		guicontroller.addCenterText ("Score: " + score);
		this.hideLeaderboard ();
		yield return new WaitForSeconds (3.0f);
		this.showLeaderboard ();

		scoreObj.SetActive (true);
		scoreObj.GetComponent<TextMesh> ().text = "Score: " + score;

		if (isFirebase) {
			if (isEntryHighScore (tempScore)) {
				// Input Name for new highscore
				// When Input complete,
				// show score
				// show main menu button

				nameInputObj.SetActive (true);
				leaderboardName.GetComponent<TextMesh> ().text = leaderboardNameString;

				while(!isInputName)
				{
					yield return new WaitForSeconds(0.5f);
				}

				nameInputObj.SetActive (false);

				string leaderboardText = "";
				for (int i = 1; i <= 10; i++) {
					leaderboardText += i + ": " + leaderboard [i - 1].name + " | " + leaderboard [i - 1].score + "\n";
				}

				leaderboardTitleObj.SetActive (true);
				leaderboardObj.SetActive (true);
				leaderboardObj.GetComponent<TextMesh> ().text = leaderboardText;
				returnToMainMenuObj.SetActive(true);
			} else {
				// Show score
				// Show highscore list
				// Show Main menu button

				string leaderboardText = "";
				leaderboardText = "    Leaderboard\n\n";

				for (int i = 1; i <= 10; i++) {
					leaderboardText += i + ": " + leaderboard [i - 1].name + " | " + leaderboard [i - 1].score + "\n";
				}

				leaderboardTitleObj.SetActive (true);
				leaderboardObj.SetActive (true);
				leaderboardObj.GetComponent<TextMesh> ().text = leaderboardText;
				returnToMainMenuObj.SetActive(true);
			}
		} else {
			// Show unable to get high score list
			// Prompt return to main menu only
			unavailableObj.SetActive(true);
			returnToMainMenuObj.SetActive (true);
		}
	}

	public bool isEntryHighScore(int tempScore)
	{
		if (isFirebase && tempScore > leaderboard [9].score)
			return true;
		else
			return false;
	}

	void OnEnable()
	{
		Firebase_Leaderboard_Controller.OnDataReceived += this.OnEventDataReceived;
		Firebase_Leaderboard_Controller.OnDataTimeout += this.OnEventDataTimeOut;
		Gaze_Controller.GazeInputKeyboard += OnKeyboardGaze;
	}

	void OnDisable()
	{
		Firebase_Leaderboard_Controller.OnDataReceived -= this.OnEventDataReceived;
		Firebase_Leaderboard_Controller.OnDataTimeout -= this.OnEventDataTimeOut;
		Gaze_Controller.GazeInputKeyboard -= OnKeyboardGaze;
	}
		
	void OnEventDataReceived(Firebase_Leaderboard_Controller.LeaderboardEntry[] data)
	{
		Debug.Log ("Data Received to leaderboard manager." + data.ToString ());
		leaderboard = (Firebase_Leaderboard_Controller.LeaderboardEntry[]) data.Clone ();
		for (int i = 1; i <= 10; i++) {
			Debug.Log (i + ": " + data[i-1].name + " | " + data[i-1].score);
		}

		isFirebase = true;
	}

	void OnEventDataTimeOut(Firebase_Leaderboard_Controller.LeaderboardEntry[] data)
	{
		Debug.Log ("Data Not Received to leaderboard manager b/c timeout");

		isFirebase = false;
	}





}
