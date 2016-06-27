using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Round_Manager : MonoBehaviour {
	// Events
	public delegate void Round();
	public static event Round OnRoundOver;
	public static event Round OnGameStart;
	public static event Round OnGameOver;

	// Round States
	public enum ROUND_STATE{END, START, PLAY};
	public ROUND_STATE state = ROUND_STATE.END;

	// Round Data
	private Round_Data.ROUND_VARIABLES[] roundData;

	// Round variables, used by GUI
	public int roundNum = 0;
	public int animalsSaved = 0;
	public int animalsNeeded = 100;
	public float timeStarted = 0.0f;
	public float timeRoundRemaining = 0.0f;
	public float timeEnding = 0.0f;
	public float timeRoundDuration = 90.0f;
	public int points = 0;
	int roundMax = 10;

	public bool isPowerup = false;
	public bool isBomb = false;
	public bool isTractor = false;
	public bool isHay = false;

	public GameObject guiObj;
	private GUI_Controller guiController;
	public GameObject playerObj;
	private Player_Controller_VR playerController;

	//spawner variables
	public bool isSpawnPowerups = false;

	public int animalSpawnIndexer = 0;
	public GameObject[] animalSpawnLocations;
	public GameObject cowPrefab;
	public GameObject chickenPrefab;
	public GameObject pigPrefab;
	public int powerupSpawnIndexer = 0;
	public GameObject[] powerupSpawnLocations;
	public GameObject bombPrefab;
	public GameObject tractorPrefab;
	public GameObject hayPrefab;
	public int alienSpawnIndexer = 0;
	public GameObject[] alienSpawnLocations;
	public GameObject ufoPrefab;

	public GameObject leaderboardManagerObj;
	public Leaderboard_Manager leaderboardManager;

	// Use this for initialization
	void Start ()
	{
		guiController = guiObj.GetComponent<GUI_Controller> ();
		guiController.syncWithRoundManager = true;
		guiController.hideCanvas ();
		//GUICONTROLLER addCenterText, setCenterTextInterval, addPowerup, hideCanvas, showCanvas

		leaderboardManager = leaderboardManagerObj.GetComponent<Leaderboard_Manager> ();

		playerController = playerObj.GetComponent<Player_Controller_VR> ();
		this.playerController.setPlayerSpeed (0.0f);

		this.roundData = this.GetComponent<Round_Data> ().roundData;

		animalSpawnLocations = GameObject.FindGameObjectsWithTag ("Animal Spawn");
		powerupSpawnLocations = GameObject.FindGameObjectsWithTag ("Powerup Spawn");
		alienSpawnLocations = GameObject.FindGameObjectsWithTag ("Alien Spawn");

		timeStarted = Time.time;
		timeEnding = timeStarted + timeRoundDuration;

		StartCoroutine ("PowerupSpawnerCoroutine");

		//this.startRound (1);
	}

	// Event listeners enable and disable
	// Set Event Listeners on enable
	void OnEnable()
	{
		Powerup_Controller.OnHayPowerUp += EventPowerupHay;
		Powerup_Controller.OnBombPowerUp += EventPowerupBomb;
		Powerup_Controller.OnTractorPowerUp += EventPowerupTractor;
		Animal_Controller.OnAnimalSaved += EventAnimalSaved;
		Animal_Controller.OnAnimalCaptured += EventAnimalCaptured;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Powerup_Controller.OnHayPowerUp -= EventPowerupHay;
		Powerup_Controller.OnBombPowerUp -= EventPowerupBomb;
		Powerup_Controller.OnTractorPowerUp -= EventPowerupTractor;
		Animal_Controller.OnAnimalSaved -= EventAnimalSaved;
		Animal_Controller.OnAnimalCaptured -= EventAnimalCaptured;
	}
	
	// Update is called once per frame
	void Update () {
		timeRoundRemaining = timeEnding - Time.time;

		if (timeRoundRemaining <= 0 && state == ROUND_STATE.PLAY) {
			StartCoroutine (EndGameCoroutine());
		}
	}

	public void EventAnimalSaved(Animal_Controller.ANIMAL_TYPE type)
	{
		// Add Points depending on animal
		// Change Animals Saved
		// Check if equals Animals Needed
		// if so, then enter round over state

		switch (type) {
		case Animal_Controller.ANIMAL_TYPE.COW:
			points += 10;
			break;
		case Animal_Controller.ANIMAL_TYPE.PIG:
			points += 25;
			break;
		case Animal_Controller.ANIMAL_TYPE.CHICKEN:
			points += 50;
			break;
		default:
			break;
		}

		animalsSaved++;

		if (animalsSaved >= animalsNeeded) {
			// Change state to round over
			// Start coroutine
			// Do only if state is in play, then call method end round given round number
			if (state == ROUND_STATE.PLAY) {
				StartCoroutine ("EndRoundCoroutine");
			}
		}
	}

	public void EventAnimalCaptured(Animal_Controller.ANIMAL_TYPE type)
	{
		// subtract points?
		// add multiplier when chaining animal saving?
	}

	public void EventPowerupHay(float time)
	{
		points += 50;
		//Set all animal agent destination to player for certain time
		guiController.addPowerup("Hay", time);
	}

	public void EventPowerupBomb(float time)
	{
		// Stun aliens and make them drop animal if captured
		points += 50;
		guiController.addPowerup ("Bomb", time);
	}

	public void EventPowerupTractor(float time)
	{
		points += 50;
		//Increase player speed for a certain time
		guiController.addPowerup("Tractor", time);
	}
		
	public void startRound(int roundNumber)
	{
		if (roundNumber == 1) {
			OnGameStart ();
		}


		roundNum = roundNumber;

		if (roundNum > roundMax) {
			StartCoroutine ("EndGameCoroutine");
		}

		guiController.showCanvas ();
		StartCoroutine ("StartRoundCoroutine");
	}

	IEnumerator StartRoundCoroutine()
	{
		state = ROUND_STATE.START;

		guiController.addCenterText ("ROUND " + roundNum);
		guiController.addCenterText ("START!");

		yield return new WaitForSeconds (2.0f);

		// Spawn powerups, animals, and animals given round number
		// Set Time, Animals Saved, Animals Required
		isSpawnPowerups = true;
		spawnPowerups(3);
		spawnAliens (roundData[roundNum-1].aliensSpawned);
		spawnAnimals (roundData[roundNum-1].animalsSpawned);
		this.animalsNeeded = roundData [roundNum - 1].animalsNeeded;
		this.animalsSaved = 0;
		this.timeStarted = Time.time;
		this.timeRoundDuration = roundData [roundNum - 1].roundDuration;
		this.timeEnding = timeStarted + timeRoundDuration;

		StartCoroutine ("PlayRoundCoroutine");
	}

	IEnumerator PlayRoundCoroutine()
	{
		state = ROUND_STATE.PLAY;
		playerController.setPlayerSpeed (8.0f);
		yield break;
	}

	IEnumerator EndRoundCoroutine()
	{
		OnRoundOver ();

		state = ROUND_STATE.END;
		//Stop player movement if end of round
		playerController.setPlayerSpeed (0.0f);
		guiController.addCenterText ("ROUND OVER");

		isSpawnPowerups = false;
		yield return new WaitForSeconds (1.0f);

		if (roundNum < roundMax) {
			startRound ((roundNum + 1));
		} else
			StartCoroutine ("EndGameCoroutine");
	}

	IEnumerator EndGameCoroutine()
	{
		state = ROUND_STATE.END;
		OnGameOver ();
		OnRoundOver ();
		playerController.setPlayerSpeed (0.0f);

		if (roundNum >= roundMax) {
			guiController.addCenterText ("FINISHED ALL " + roundMax + " ROUNDS!");
		} else {
			guiController.addCenterText ("Time up!");
		}

		yield return new WaitForSeconds (1.0f);
		leaderboardManager.StartCoroutine (leaderboardManager.StartEndGameLeaderboard(points));
	}

	public void spawnAnimals(int num)
	{
		for (int i = 0; i < num; i++) {
			Vector3 spawnlocation = animalSpawnLocations[animalSpawnIndexer % 10].transform.position;

			switch (animalSpawnIndexer % 3) {
			case 0:
				Instantiate (cowPrefab,spawnlocation, Quaternion.identity);
				break;
			case 1:
				Instantiate (pigPrefab,spawnlocation, Quaternion.identity);
				break;
			case 2:
				Instantiate (chickenPrefab,spawnlocation, Quaternion.identity);
				break;
			default:
				break;
			}

			animalSpawnIndexer++;
		}	
	}

	public void spawnAliens(int num)
	{
		for (int i = 0; i < num; i++) {
			Vector3 spawnlocation = alienSpawnLocations[alienSpawnIndexer % 5].transform.position;
			Instantiate (ufoPrefab,spawnlocation, Quaternion.identity);
			alienSpawnIndexer++;
		}	
	}

	public void spawnPowerups(int num)
	{
		if (isSpawnPowerups) {
			for (int i = 0; i < num; i++) {
				Vector3 spawnlocation = powerupSpawnLocations [(powerupSpawnIndexer % 5)].transform.position;
				switch (powerupSpawnIndexer % 3) {
				case 0:
					Instantiate (bombPrefab, spawnlocation, Quaternion.identity);
					break;
				case 1:
					Instantiate (tractorPrefab, spawnlocation, Quaternion.identity);
					break;
				case 2:
					Instantiate (hayPrefab, spawnlocation, Quaternion.identity);
					break;
				default:
					break;
				}

				powerupSpawnIndexer++;
			}	
		}
	}

	IEnumerator PowerupSpawnerCoroutine()
	{
		while (true) {
			yield return new WaitForSeconds (20.0f);
			if (isSpawnPowerups) {
				spawnPowerups (1);
			}
		}
	}


}
