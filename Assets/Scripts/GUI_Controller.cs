using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUI_Controller : MonoBehaviour
{
	// Reference to Round Manager
	public GameObject roundManagerObj;
	private Round_Manager roundManager;

	//Reference to gui elements
	public GameObject roundObj;
	public GameObject timeObj;
	public GameObject pointsObj;
	public GameObject animalsObj;
	public GameObject centerObj;
	public GameObject powerupNameObj;
	public GameObject powerupSliderObj;

	//private references to gui components
	private Text roundText;
	private Text timeText;
	private Text pointsText;
	private Text animalsText;
	private Text centerText;
	private Text powerupText;
	private Slider powerupSlider;

	//flags
	public bool enableCanvas = false;
	public bool syncWithRoundManager = false;

	void Awake()
	{
		roundManager = roundManagerObj.GetComponent<Round_Manager> ();

		roundText = roundObj.GetComponent<Text> ();
		timeText = timeObj.GetComponent<Text> ();
		pointsText = pointsObj.GetComponent<Text> ();
		animalsText = animalsObj.GetComponent<Text> ();
		centerText = centerObj.GetComponent<Text> ();
		powerupText = powerupNameObj.GetComponent<Text> ();
		powerupSlider = powerupSliderObj.GetComponent<Slider> ();
	}
	// Use this for initialization
	void Start ()
	{
		StartCoroutine ("CenterTextCoroutine");
	}
	
	// Update is called once per frame
	void Update ()
	{
		// do updates for each gui element
		// if sync is true, get gui values from round manager
		if (enableCanvas && syncWithRoundManager) {
			roundText.text = "Round " + roundManager.roundNum;

			// time Text logic, given floats to 00:00 format
			int timeLeft = (int)roundManager.timeRoundRemaining;
			int minutes = (int) Mathf.Floor ( ((float)timeLeft) / 60.0f );
			int seconds = timeLeft - minutes * 60;

			timeText.text = "";

			if (minutes.ToString ().Length == 1) {
				timeText.text += "0";
				timeText.text += minutes.ToString ();
			} else {
				timeText.text += minutes.ToString ();
			}

			timeText.text += ":";

			if (seconds.ToString ().Length == 1) {
				timeText.text += "0";
				timeText.text += seconds.ToString ();
			} else {
				timeText.text += seconds.ToString ();
			}

			pointsText.text = roundManager.points.ToString();
			animalsText.text = roundManager.animalsSaved + " / " + roundManager.animalsNeeded;
			// CENTER TEXT IS NOT HANDLED HERE, BUT BY COROUTINES CALLED BY MANAGER
			// POWERUP TEXT IS NOT HANDLED HERE, BUT ALSO BY COROUTINES CALLED BY MANAGER
			if (!isPowerup) {
				powerupText.enabled = false;
				powerupSliderObj.SetActive (false);
			}
		}
	}

	//CENTER TEXT, KEEP A QUEUE OF STRINGS TO DISPLAY
	//AFTER CENTER TEXT INTERVAL, DEQUEUE STRING AND DISPLAY NEXT STRING
	//IF NO STRINGS IN QUEUE, DISABLE CENTER TEXT
	Queue<string> centerTextQueue = new Queue<string>();
	public float centerTextInterval = 1.0f;
	public string centerTextDisplayed = "null";

	public void addCenterText(string textToAdd)
	{
		centerTextQueue.Enqueue (textToAdd);
	}

	public void setCenterTextInterval(float intervalToSet)
	{
		centerTextInterval = intervalToSet;
	}

	IEnumerator CenterTextCoroutine()
	{
		while(true)
		{
			yield return new WaitForSeconds (centerTextInterval);

			if (centerText != null) {
				if (centerTextQueue.Count > 0) {
					centerTextDisplayed = centerTextQueue.Dequeue ();
					centerText.text = centerTextDisplayed;
				} else {
					centerText.text = "";
				}
			}
		}
	}

	//POWERUP SLIDER AND TEXT
	//POWERUP SLIDER AND TEXT ARE DISABLED AT START
	//CALL ENABLES A COROUTINE THAT ENABLES THEM
	//AFTER COROUTINE FINISHES, THEY ARE DISABLED AGAIN
	//IF CALL DURING COROUTINE, THEN CLEAR VALUES AND OVERRIDE WITH NEW COROUTINE AND VALUES
	public bool isPowerup = false;
	public float powerupDur = 5.0f;
	public string powerupNameToDisplay = "null";
	public float powerupStartTime = 0.0f;

	public void addPowerup(string powerupName, float duration)
	{
		StopCoroutine ("PowerupCoroutine");	
		powerupNameToDisplay = powerupName;
		powerupDur = duration;

		powerupText.text = powerupNameToDisplay;
		powerupSlider.maxValue = powerupDur;
		powerupStartTime = Time.time;

		StartCoroutine ("PowerupCoroutine");
	}

	IEnumerator PowerupCoroutine()
	{
		isPowerup = true;
		powerupText.enabled = true;
		powerupSliderObj.SetActive (true);

		while(Time.time < powerupStartTime + powerupDur)
		{
			yield return new WaitForSeconds (0.01f);
			powerupSlider.value = 5 - (Time.time - powerupStartTime);
		}

		isPowerup = false;
		powerupText.enabled = false;
		powerupSliderObj.SetActive (false);
		powerupNameToDisplay = "null";

		yield return new WaitForSeconds (0.01f);
	}


	public void showCanvas()
	{
		// enable gui elements
		enableCanvas = true;
		roundText.enabled = true;
		timeText.enabled = true;
		pointsText.enabled = true;
		animalsText.enabled = true;
		centerText.enabled = true;
		powerupText.enabled = true;
		powerupSliderObj.SetActive (true);
	}

	public void hideCanvas()
	{
		// hide all canvas elements
		enableCanvas = false;
		roundText.enabled = false;
		timeText.enabled = false;
		pointsText.enabled = false;
		animalsText.enabled = false;
		centerText.enabled = false;
		powerupText.enabled = false;
		powerupSliderObj.SetActive (false);
		// cancel all coroutines
		//	e.g. center text
		//	clear center text queue
		centerTextQueue.Clear();

	}




}
