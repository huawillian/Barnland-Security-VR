using UnityEngine;
using System.Collections;

public class Player_Controller_VR : MonoBehaviour
{
	// Reference to Round Manager
	public GameObject roundManagerObj;
	private Round_Manager roundManager;

	// Reference to Player Navigation Controller
	public GameObject headObj;

	public float playerSpeed = 6.0f;
	public float playerTractorSpeed = 12.0f;
	public float speed = 6.0f;

	// COLLIDE WITH POWERUP CALL ROUNDMANAGER POWERUP METHOD
	// COLLIDE WITH ANIMAL CALL ROUNDMANAGER ANIMAL METHOD
	// MUST EXPOSE player speed attribute

	public Vector3 direction;

	// Use this for initialization
	void Start () {
		roundManager = roundManagerObj.GetComponent<Round_Manager> ();
	}

	void Update() {
		Vector3 newVel = headObj.gameObject.transform.forward * speed;
		newVel = new Vector3 (newVel.x, 0, newVel.z);
		this.GetComponent<Rigidbody> ().velocity = newVel;
	}

	public void setPlayerSpeed(float spd)
	{
		playerSpeed = spd;
		playerTractorSpeed = 2.0f * spd;
		speed = playerSpeed;
	}

	// Event listeners enable and disable
	// Set Event Listeners on enable
	void OnEnable()
	{
		Powerup_Controller.OnTractorPowerUp += EventPowerupTractor;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Powerup_Controller.OnTractorPowerUp -= EventPowerupTractor;
	}

	private void EventPowerupTractor(float time)
	{
		this.StopAllCoroutines ();
		StartCoroutine (setTractorPowerUp(time));
	}

	private IEnumerator setTractorPowerUp(float time)
	{
		speed = playerTractorSpeed;
		yield return new WaitForSeconds (time);
		speed = playerSpeed;
	}


}
