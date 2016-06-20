using UnityEngine;
using System.Collections;

public class Round_Manager : MonoBehaviour {
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

	public bool isPowerup = false;
	public bool isBomb = false;
	public bool isTractor = false;
	public bool isHay = false;
	public float powerupDuration = 5.0f;

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


	// Use this for initialization
	void Start ()
	{
		guiController = guiObj.GetComponent<GUI_Controller> ();
		guiController.syncWithRoundManager = true;
		guiController.hideCanvas ();
		//GUICONTROLLER addCenterText, setCenterTextInterval, addPowerup, hideCanvas, showCanvas

		playerController = playerObj.GetComponent<Player_Controller_VR> ();
		//setPlayerSpeed

		this.roundData = this.GetComponent<Round_Data> ().roundData;

		animalSpawnLocations = GameObject.FindGameObjectsWithTag ("Animal Spawn");
		powerupSpawnLocations = GameObject.FindGameObjectsWithTag ("Powerup Spawn");
		alienSpawnLocations = GameObject.FindGameObjectsWithTag ("Alien Spawn");

		timeStarted = Time.time;
		timeEnding = timeStarted + timeRoundDuration;

		StartCoroutine ("PowerupSpawnerCoroutine");

		this.startRound (1);
	}
	
	// Update is called once per frame
	void Update () {
		timeRoundRemaining = timeEnding - Time.time;

		//Stop player movement if end of round
		if (state != ROUND_STATE.PLAY) {
			playerController.setPlayerSpeed (0.0f);
		}
	}

	public void eventCollideAnimal(string animalType)
	{
		// Add Points depending on animal
		// Change Animals Saved
		// Check if equals Animals Needed
		// if so, then enter round over state

		switch (animalType) {
		case "Cow":
			points += 10;
			break;
		case "Pig":
			points += 25;
			break;
		case "Chicken":
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

	public void eventCollidePowerup(string powerupType)
	{
		// Add Points
		// Powerup depending on the powerup
		// send message to GUI Controller

		points += 50;

		switch (powerupType) {
		case "Bomb":
			// Lower Speed of all aliens for a certain time
			// Call coroutine
			StopCoroutine("BombPowerupCoroutine");
			guiController.addPowerup ("Bomb", powerupDuration);
			StartCoroutine ("BombPowerupCoroutine");
			break;
		case "Tractor":
			//Increase player speed for a certain time
			//Call coroutine
			StopCoroutine("TractorPowerupCoroutine");
			guiController.addPowerup("Tractor", powerupDuration);
			StartCoroutine ("TractorPowerupCoroutine");
			break;
		case "Hay":
			//Set all animal agent destination to player for certain time
			//Call coroutine
			StopCoroutine("HayPowerupCoroutine");
			guiController.addPowerup("Hay", powerupDuration);
			StartCoroutine ("HayPowerupCoroutine");
			break;
		default:
			break;
		}
	}

	IEnumerator BombPowerupCoroutine()
	{
		GameObject[] aliens = GameObject.FindGameObjectsWithTag ("Alien");

		if (aliens.Length == 0) {
			yield break;
		}

		foreach (GameObject obj in aliens) {
			if(obj)
			{
				Alien_Controller controller = obj.GetComponent<Alien_Controller> ();
				controller.moveSpeed = 0.0f;
			}
		}

		yield return new WaitForSeconds (powerupDuration);

		foreach (GameObject obj in aliens) {
			if(obj)
			{
				Alien_Controller controller = obj.GetComponent<Alien_Controller> ();
				controller.moveSpeed = 5.0f;
			}
		}
	}

	IEnumerator TractorPowerupCoroutine()
	{
		playerController.setPlayerSpeed (24.0f);

		yield return new WaitForSeconds (powerupDuration);

		playerController.setPlayerSpeed (8.0f);
	}

	IEnumerator HayPowerupCoroutine()
	{
		float timeStart = Time.time;

		GameObject[] animals = GameObject.FindGameObjectsWithTag ("Animal");

		while (Time.time < timeStart + powerupDuration) {
			foreach (GameObject obj in animals) {
				if (obj) {
					obj.GetComponent<Animal_Controller> ().setDestinationtoPlayer (playerController.gameObject.transform.position);
				}
			}
			yield return new WaitForSeconds (0.01f);
		}
			
		yield break;
	}

	public void startRound(int roundNumber)
	{
		roundNum = roundNumber;
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
		state = ROUND_STATE.END;

		guiController.addCenterText ("ROUND OVER");

		GameObject[] aliens = GameObject.FindGameObjectsWithTag ("Alien");
		GameObject[] animals = GameObject.FindGameObjectsWithTag ("Animal");
		GameObject[] powerups = GameObject.FindGameObjectsWithTag ("Powerup");

		foreach (GameObject obj in aliens)
			Destroy (obj);

		foreach (GameObject obj in animals)
			Destroy (obj);

		foreach (GameObject obj in powerups)
			Destroy (obj);


		isSpawnPowerups = false;

		yield return new WaitForSeconds (1.0f);

		startRound ((roundNum + 1));
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
