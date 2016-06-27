using UnityEngine;
using System.Collections;

public class Player_SoundController : MonoBehaviour
{
	private AudioSource bombSource;
	private AudioSource chickenSource;
	private AudioSource cowSource;
	private AudioSource cowbellSource;
	private AudioSource endingSource;
	private AudioSource menuSource;
	private AudioSource pigSource;
	private AudioSource playSource;
	private AudioSource tractorSource;

	public AudioClip bombClip;
	public AudioClip chickenClip;
	public AudioClip cowClip;
	public AudioClip cowbellClip;
	public AudioClip endingClip;
	public AudioClip menuClip;
	public AudioClip pigClip;
	public AudioClip playClip;
	public AudioClip tractorClip;


	//TODO: (- checked)
	// - When game starts, then play menu source
	// - When round starts, then stop menu source
	// - When round starts, then start play source
	// - When animal is saved, then play corresponding animal source
	// - When animal is captured, then play corresponding animal source
	// - When powerup is activated, then play corresponding powerup source
	// - When game over, then play ending source
	// - When game over, then stop play source
	// - When gaze select, then play cowbell
	// Randomly during game play animal sound


	// Use this for initialization
	void Start ()
	{
		bombSource = gameObject.AddComponent<AudioSource> ();
		chickenSource = gameObject.AddComponent<AudioSource> ();
		cowSource = gameObject.AddComponent<AudioSource> ();
		cowbellSource = gameObject.AddComponent<AudioSource> ();
		endingSource = gameObject.AddComponent<AudioSource> ();
		menuSource = gameObject.AddComponent<AudioSource> ();
		pigSource = gameObject.AddComponent<AudioSource> ();
		playSource = gameObject.AddComponent<AudioSource> ();
		tractorSource = gameObject.AddComponent<AudioSource> ();

		bombSource.clip = bombClip;
		chickenSource.clip = chickenClip;
		cowSource.clip = cowClip;
		cowbellSource.clip = cowbellClip;
		endingSource.clip = endingClip;
		menuSource.clip = menuClip;
		pigSource.clip = pigClip;
		playSource.clip = playClip;
		tractorSource.clip = tractorClip;

		menuSource.volume = 0.25f;
		playSource.volume = 0.3f;
		endingSource.volume = 0.3f;

		// On start, play menu music
		menuSource.Play();
		menuSource.loop = true;
	}

	void OnEnable()
	{
		Round_Manager.OnGameStart += gameStart;
		Animal_Controller.OnAnimalSaved += animalSaved;
		Animal_Controller.OnAnimalCaptured += animalCaptured;
		Powerup_Controller.OnBombPowerUp += bomb;
		Powerup_Controller.OnHayPowerUp += hay;
		Powerup_Controller.OnTractorPowerUp += tractor;
		Round_Manager.OnGameOver += gameEnd;
		Gaze_Controller.PlaySelectSound += cowbell;
	}

	void OnDisable()
	{
		Round_Manager.OnGameStart -= gameStart;
		Animal_Controller.OnAnimalSaved -= animalSaved;
		Animal_Controller.OnAnimalCaptured -= animalCaptured;
		Powerup_Controller.OnBombPowerUp -= bomb;
		Powerup_Controller.OnHayPowerUp -= hay;
		Powerup_Controller.OnTractorPowerUp -= tractor;
		Round_Manager.OnGameOver -= gameEnd;
		Gaze_Controller.PlaySelectSound -= cowbell;
	}

	void animalSaved(Animal_Controller.ANIMAL_TYPE type)
	{
		switch (type) {
		case Animal_Controller.ANIMAL_TYPE.CHICKEN:
			chickenSource.Play ();
			break;
		case Animal_Controller.ANIMAL_TYPE.COW:
			cowSource.Play ();
			break;
		case Animal_Controller.ANIMAL_TYPE.PIG:
			pigSource.Play ();
			break;
		default:
			break;
		}
	}

	void animalCaptured(Animal_Controller.ANIMAL_TYPE type)
	{
		switch (type) {
		case Animal_Controller.ANIMAL_TYPE.CHICKEN:
			chickenSource.Play ();
			break;
		case Animal_Controller.ANIMAL_TYPE.COW:
			cowSource.Play ();
			break;
		case Animal_Controller.ANIMAL_TYPE.PIG:
			pigSource.Play ();
			break;
		default:
			break;
		}
	}

	void bomb(float dur)
	{
		bombSource.Play ();
	}

	void hay(float dur)
	{
		cowbellSource.Play ();
	}

	void tractor(float dur)
	{
		tractorSource.Play ();
	}

	void gameStart()
	{
		menuSource.Stop ();

		playSource.Play ();
		playSource.loop = true;
	}

	void gameEnd()
	{
		playSource.Stop ();
		endingSource.Play ();
	}

	void cowbell()
	{
		cowbellSource.Play ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
