using UnityEngine;
using System.Collections;

// Attach to an animal object with tag Animal
// Three kinds of Animals: Cow, Pig, Chicken
// Animal is spawned from round controller

public class Animal_Controller : MonoBehaviour
{
	// animal related events
	public delegate void Animal(ANIMAL_TYPE type);
	public static event Animal OnAnimalSaved;
	public static event Animal OnAnimalCaptured;

	// type of animal
	public enum ANIMAL_TYPE{COW, PIG, CHICKEN};
	public ANIMAL_TYPE type;

	// state
	public enum ANIMAL_STATE{MOVING, LURED, BEAMED, SAVED};
	public ANIMAL_STATE state = ANIMAL_STATE.MOVING;

	// navagent
	public NavMeshAgent agent;
	public float moveTimeInterval = 5.0f;

	// alien capturing this animal
	public GameObject alien = null;
	public float captureTime = 3.0f;

	// destination object
	public GameObject destination = null;

	// player object
	public GameObject player = null;

	Coroutine hayCoroutine;

	// animation reference
	public GameObject animationObj;
	private Animation runAnimation;

	// animal sounds
	private GvrAudioSource audioSource;
	public AudioClip clip;

	// Use this for initialization
	void Start ()
	{
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		runAnimation = animationObj.GetComponent<Animation> ();

		audioSource = gameObject.AddComponent<GvrAudioSource> ();
		audioSource.clip = this.clip;
		StartCoroutine ("PlayAnimalSounds", Random.Range(10.0f, 20.0f));

		this.destination.transform.parent = this.transform.parent;
		StartCoroutine ("setStateMoving");
	}

	IEnumerator PlayAnimalSounds()
	{
		while (true) {
			if (state == ANIMAL_STATE.MOVING) {
				audioSource.Play ();
			}
			yield return new WaitForSeconds (Random.Range(10.0f, 20.0f));
		}
	}

	// Set Event Listeners on enable
	void OnEnable()
	{
		Powerup_Controller.OnHayPowerUp += OnHayEvent;
		Powerup_Controller.OnBombPowerUp += OnBombEvent;
		Round_Manager.OnRoundOver += OnRoundOverEvent;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Powerup_Controller.OnHayPowerUp -= OnHayEvent;
		Powerup_Controller.OnBombPowerUp -= OnBombEvent;
		Round_Manager.OnRoundOver -= OnRoundOverEvent;
	}
		
	// Event handlers
	public void OnHayEvent(float time)
	{
		if (state == ANIMAL_STATE.MOVING) {
			if (hayCoroutine != null) {
				StopCoroutine (hayCoroutine);
			}

			hayCoroutine = StartCoroutine (setStateLured(time));
		}
	}

	public void OnRoundOverEvent()
	{
		// Destroy this gameobject on round over
		this.StopAllCoroutines();
		Destroy(this.gameObject);
	}

	public void OnBombEvent(float time)
	{
		if (state != ANIMAL_STATE.SAVED) {
			// If state is being captured
			// then return state to moving
			// turn navmesh agent on
			this.StopAllCoroutines();
			this.agent.enabled = true;
			this.alien = null;
			StartCoroutine ("setStateMoving");
		}
	}

	// State coroutines
	private IEnumerator setStateLured(float hayDuration)
	{
		state = ANIMAL_STATE.LURED;
		runAnimation.Play ();

		this.agent.destination = this.player.transform.position;
		yield return new WaitForSeconds (hayDuration);
		hayCoroutine = null;

		if (state == ANIMAL_STATE.LURED) {
			StartCoroutine ("setStateMoving");
		}
	}

	private IEnumerator setStateMoving()
	{
		state = ANIMAL_STATE.MOVING;
		this.transform.rotation.eulerAngles.Set (0,0,0);
		this.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0,0,0);
		runAnimation.Play ();
		setRandomDestination ();
		yield return new WaitForSeconds (moveTimeInterval);
		if (state == ANIMAL_STATE.MOVING) {
			StartCoroutine ("setStateMoving");
		}
	}

	private IEnumerator setStateSaved()
	{
		state = ANIMAL_STATE.SAVED;
		runAnimation.Stop ();
		Destroy (this.gameObject);
		yield break;
	}

	private IEnumerator setStateBeamed()
	{
		state = ANIMAL_STATE.BEAMED;
		runAnimation.Stop ();
		this.agent.enabled = false;
		this.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0,3.0f,0);
		this.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (1,3,1);
		yield return new WaitForSeconds (captureTime);
		if (OnAnimalCaptured != null) {
			OnAnimalCaptured (this.type);
		}
		Destroy (this.gameObject);
		yield break;
	}

	// Trigger handlers
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == this.player.gameObject) {
			// If collide with player
			// then initiate event animal saved and pass in animal type
			// and start destroy this animal animation
			if (OnAnimalSaved != null) {
				OnAnimalSaved (this.type);
			}

			this.StopAllCoroutines ();
			StartCoroutine ("setStateSaved");
		}

		if (other.gameObject == this.destination && state == ANIMAL_STATE.MOVING) {
		// If the object is the destination, then set new destination
			this.StopAllCoroutines ();
			StartCoroutine ("setStateMoving");
		}
	}

	// Called by alien script to start capturing this animal coroutine
	public void captureAnimal(GameObject alien)
	{
		if (this.alien == null) {
			this.alien = alien;
			this.StopAllCoroutines ();
			StartCoroutine ("setStateBeamed");
		}
	}

	// helper method to set destination to random place
	private void setRandomDestination()
	{
		Vector3 currPos = this.transform.position;
		Vector3 newPos = Vector3.zero;

		do {
			newPos = new Vector3 (currPos.x + Random.Range (-15, 15), 
				currPos.y, 
				currPos.z + Random.Range (-15, 15));
		} while(!(newPos.x > -70f && newPos.x < 40f && newPos.z > -65f && newPos.z < 45f));

		destination.transform.position = newPos;
		agent.destination = newPos;
	}
}
