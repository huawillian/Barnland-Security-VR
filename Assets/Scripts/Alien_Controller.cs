using UnityEngine;
using System.Collections;

public class Alien_Controller : MonoBehaviour
{
	//state
	public enum ALIEN_STATE{SEARCHING, MOVING, CAPTURING, STUNNED};
	public ALIEN_STATE state = ALIEN_STATE.SEARCHING;

	//flags
	public float searchTimeInterval = 3.0f;
	public float moveTimeInterval = 1.0f;
	public Vector3 moveVelocity = Vector3.zero;
	public float moveSpeed = 5.0f;
	public float captureTimeInterval = 3.0f;

	//target
	public GameObject animal = null;

	//rigidbody
	public Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		this.rigidbody = this.GetComponent<Rigidbody> ();
		StartCoroutine (setStateSearching());
	}

	// State Coroutines
	private IEnumerator setStateSearching()
	{
		state = ALIEN_STATE.SEARCHING;

		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Animal");

		if (objs.Length == 0) {
			Destroy (this.gameObject);
		} else {
			animal = objs [Random.Range (0, objs.Length)];
		}

		yield return new WaitForSeconds (searchTimeInterval);

		if (state == ALIEN_STATE.SEARCHING && animal != null) {
			StartCoroutine (setStateMoving());
		} else {
			StartCoroutine (setStateSearching());
		}
			
		yield break;
	}

	private IEnumerator setStateMoving()
	{
		state = ALIEN_STATE.MOVING;

		while (true) {
			if (state == ALIEN_STATE.MOVING) {
				if (animal != null) {
					setVelocityTowardsAnimal ();
				} else {
					StartCoroutine (setStateSearching ());
					break;
				}
			}
			yield return new WaitForSeconds (moveTimeInterval);
		}
			
		yield break;
	}

	private IEnumerator setStateCapturing()
	{
		state = ALIEN_STATE.CAPTURING;
		this.rigidbody.velocity = Vector3.zero;

		yield return new WaitForSeconds (captureTimeInterval);

		if (state == ALIEN_STATE.CAPTURING) {
			StartCoroutine (setStateSearching());
		}

		yield break;
	}

	private IEnumerator setStateStunned(float time)
	{
		state = ALIEN_STATE.STUNNED;
		this.rigidbody.velocity = Vector3.zero;
		yield return new WaitForSeconds (time);

		StartCoroutine (setStateSearching());

		yield break;
	}

	// Set Event Listeners on enable
	void OnEnable()
	{
		Powerup_Controller.OnBombPowerUp += OnBombEvent;
		Round_Manager.OnRoundOver += OnRoundOverEvent;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Powerup_Controller.OnBombPowerUp -= OnBombEvent;
		Round_Manager.OnRoundOver -= OnRoundOverEvent;
	}

	// Event handlers
	public void OnRoundOverEvent()
	{
		// Destroy this gameobject on round over
		this.StopAllCoroutines();
		Destroy(this.gameObject);
	}

	public void OnBombEvent(float time)
	{
		if (state != ALIEN_STATE.STUNNED) {
			this.StopAllCoroutines ();
			StartCoroutine (setStateStunned (time));
		}
	}

	// On collide wwith animal
	void OnTriggerEnter(Collider other)
	{
		if (state == ALIEN_STATE.MOVING && other.gameObject.tag.Equals("Animal") && other.gameObject.GetComponent<Animal_Controller>().alien == null) {
			animal = other.gameObject;
			this.transform.position = new Vector3 (animal.transform.position.x, this.transform.position.y, animal.transform.position.z);
			animal.GetComponent<Animal_Controller> ().captureAnimal (this.gameObject);
			StartCoroutine (setStateCapturing ());
		}
	}

	private void setVelocityTowardsAnimal()
	{
		Vector3 offset = animal.transform.position - this.transform.position + new Vector3(0,2,0);
		moveVelocity = Vector3.zero;

		if (offset.x > 0) {
			moveVelocity = new Vector3 (moveSpeed, 0, moveVelocity.z);
		} else {
			moveVelocity = new Vector3 (-1.0f * moveSpeed, 0, moveVelocity.z);
		}

		if (offset.z > 0) {
			moveVelocity = new Vector3 (moveVelocity.x, 0, moveSpeed);
		} else {
			moveVelocity = new Vector3 (moveVelocity.x, 0, -1.0f * moveSpeed);
		}

		if (offset.y > 0) {
			moveVelocity = new Vector3 (moveVelocity.x, moveSpeed, moveVelocity.z);
		} else {
			moveVelocity = new Vector3 (moveVelocity.x, -1.0f * moveSpeed, moveVelocity.z);
		}

		if (this.transform.position.y <= 0) {
			moveVelocity = new Vector3 (moveVelocity.x, 0, moveVelocity.z);
		}

		this.rigidbody.velocity = moveVelocity;
	}

}
