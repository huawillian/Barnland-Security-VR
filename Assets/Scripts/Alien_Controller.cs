using UnityEngine;
using System.Collections;

public class Alien_Controller : MonoBehaviour
{
	//type of alien
	public enum ALIEN_TYPE{UFO};
	public ALIEN_TYPE type;

	//state
	public enum ALIEN_STATE{SEARCHING, MOVING, CAPTURING, EXITING};
	public ALIEN_STATE state = ALIEN_STATE.SEARCHING;

	//flags
	public bool searchSet = false;
	public float searchTimeInterval = 3.0f;
	public float searchTime = 0.0f;

	public float moveTimeInterval = 1.0f;
	public float moveTime = 0.0f;
	public Vector3 moveVelocity = Vector3.zero;
	public float moveSpeed = 5.0f;

	public bool captureSet = false;
	public float captureTime = 0.0f;
	public float captureTimeInterval = 5.0f;

	public Vector3 exitPoint = Vector3.zero;
	public bool exitSet = false;

	//target
	public GameObject animal;

	//rigidbody
	public Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		this.rigidbody = this.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (state == ALIEN_STATE.SEARCHING) {
			if (!searchSet) {
				// Set Animal to pursue
				searchSet = true;
				searchTime = Time.time;
				this.rigidbody.velocity = Vector3.zero;

				GameObject[] objs = GameObject.FindGameObjectsWithTag ("Animal");
				if (objs.Length == 0) {
					Destroy (this.gameObject);
				} else {
					animal = objs [Random.Range (0, objs.Length)];
				}
			} else if (Time.time > searchTime + searchTimeInterval) {
				// If search time up, move to next state
				state = ALIEN_STATE.MOVING;
			} else if (animal == null) {
				// If not time up and animal is gone, then redo search
				searchSet = false;
			} else {
				// Wait until search time completed
			}
		}

		if (state == ALIEN_STATE.MOVING) {
			if (animal == null) {
				// If animal is not set, return to search state
				searchSet = false;
				state = ALIEN_STATE.SEARCHING;
			} else if (Time.time > moveTime + moveTimeInterval) {
				// If past move time interval, set new velocity
				moveTime = Time.time;
				Vector3 offset = animal.transform.position -  this.transform.position;
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
			} else {
				// If not past move time interval, do nothing
			}
		}

		if (state == ALIEN_STATE.CAPTURING) {
			if (animal == null) {
				// If animal is not set, return to search state
				searchSet = false;
				state = ALIEN_STATE.SEARCHING;
			} else if (!captureSet) {
				// If capture is set, then begin animating
				captureSet = true;
				captureTime = Time.time;
				animal.transform.parent = this.transform;
			} else if (Time.time > captureTime + captureTimeInterval) {
				// If past capture time interval, then start exiting
				this.rigidbody.velocity = Vector3.zero;
				state = ALIEN_STATE.EXITING;
			} else {
				// Animate capturing
				this.rigidbody.velocity = Vector3.up;
			}
		}

		if (state == ALIEN_STATE.EXITING) {
			if (animal == null) {
				// If animal is not set, return to search state
				searchSet = false;
				state = ALIEN_STATE.SEARCHING;
			} else if (!exitSet) {
				// If exit not set, then set Exit Point
				exitSet = true;
				exitPoint = new Vector3 (this.transform.position.x + Random.Range (-30, 30),
					this.transform.position.y + 30,
					this.transform.position.z + Random.Range (-30, 30));
			} else if((exitPoint - this.transform.position).magnitude < 1.0f) {
				// If close to exit point, then destroy animal and return to search
				this.rigidbody.velocity = Vector3.zero;
				Destroy(animal);
			} else {
				// If not reach exit point, then move towards it
				Vector3 offset = exitPoint - this.transform.position;
				moveVelocity = Vector3.zero;

				if (offset.x > 0) {
					moveVelocity = new Vector3 (moveSpeed, moveVelocity.y, moveVelocity.z);
				} else {
					moveVelocity = new Vector3 (-1.0f * moveSpeed, moveVelocity.y, moveVelocity.z);
				}

				if (offset.z > 0) {
					moveVelocity = new Vector3 (moveVelocity.x, moveVelocity.y, moveSpeed);
				} else {
					moveVelocity = new Vector3 (moveVelocity.x, moveVelocity.y, -1.0f * moveSpeed);
				}

				if (offset.y > 0) {
					moveVelocity = new Vector3 (moveVelocity.x, moveSpeed, moveVelocity.z);
				} else {
					moveVelocity = new Vector3 (moveVelocity.x, -1.0f * moveSpeed, moveVelocity.z);
				}

				this.rigidbody.velocity = moveVelocity;
			}
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (state == ALIEN_STATE.MOVING && other.gameObject.tag.Equals("Animal") && other.gameObject.GetComponent<Animal_Controller>().alien == null) {
			animal = other.gameObject;
			animal.GetComponent<Animal_Controller> ().disableAnimal ();
			animal.GetComponent<Animal_Controller> ().alien = this.gameObject;
			this.rigidbody.velocity = Vector3.zero;
			captureSet = false;
			state = ALIEN_STATE.CAPTURING;
		}
	}
}
