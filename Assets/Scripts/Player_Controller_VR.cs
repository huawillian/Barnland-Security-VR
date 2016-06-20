using UnityEngine;
using System.Collections;

public class Player_Controller_VR : MonoBehaviour
{
	// Reference to Round Manager
	public GameObject roundManagerObj;
	private Round_Manager roundManager;

	// Reference to Player Navigation Controller
	public GameObject headObj;

	public float playerSpeed = 8.0f;

	// COLLIDE WITH POWERUP CALL ROUNDMANAGER POWERUP METHOD
	// COLLIDE WITH ANIMAL CALL ROUNDMANAGER ANIMAL METHOD
	// MUST EXPOSE player speed attribute

	public Vector3 direction;

	// Use this for initialization
	void Start () {
		roundManager = roundManagerObj.GetComponent<Round_Manager> ();
	}

	void Update() {
		Vector3 newVel = headObj.gameObject.transform.forward * playerSpeed * 1.5f;
		newVel = new Vector3 (newVel.x, 0, newVel.z);
		this.GetComponent<Rigidbody> ().velocity = newVel;
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;
		string objTag = obj.tag;

		if (objTag.Equals("Animal")) {
			switch (obj.GetComponent<Animal_Controller> ().type) {
			case Animal_Controller.ANIMAL_TYPE.CHICKEN:
				roundManager.eventCollideAnimal ("Chicken");
				break;
			case Animal_Controller.ANIMAL_TYPE.PIG:
				roundManager.eventCollideAnimal ("Pig");
				break;
			case Animal_Controller.ANIMAL_TYPE.COW:
				roundManager.eventCollideAnimal ("Cow");
				break;
			default:
				break;
			}

			Destroy (other.gameObject);
		}

		if (objTag.Equals ("Powerup")) {
			switch (obj.GetComponent<Powerup_Controller> ().type) {
			case Powerup_Controller.POWERUP_TYPE.BOMB:
				roundManager.eventCollidePowerup ("Bomb");
				break;
			case Powerup_Controller.POWERUP_TYPE.HAY:
				roundManager.eventCollidePowerup ("Hay");
				break;
			case Powerup_Controller.POWERUP_TYPE.TRACTOR:
				roundManager.eventCollidePowerup ("Tractor");
				break;
			default:
				break;
			}

			Destroy (other.gameObject);
		}

	}

	public void setPlayerSpeed(float speed)
	{
		playerSpeed = speed;
	}


}
