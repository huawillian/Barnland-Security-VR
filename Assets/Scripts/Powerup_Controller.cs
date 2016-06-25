using UnityEngine;
using System.Collections;

public class Powerup_Controller : MonoBehaviour
{
	// Events
	public delegate void PowerUp(float duration);
	public static event PowerUp OnHayPowerUp;
	public static event PowerUp OnBombPowerUp;
	public static event PowerUp OnTractorPowerUp;

	//type of powerup
	public enum POWERUP_TYPE{BOMB, TRACTOR, HAY};
	public POWERUP_TYPE type;

	public float powerupDuration = 5.0f;

	// Temphay spawn after getting hay powerup
	public GameObject tempHayPrefab;

	// Set Event Listeners on enable
	void OnEnable()
	{
		Round_Manager.OnRoundOver += OnRoundOverEvent;
	}

	// Remove event listeners on disable
	void OnDisable()
	{
		Round_Manager.OnRoundOver -= OnRoundOverEvent;
	}

	private void OnRoundOverEvent()
	{
		Destroy (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		this.gameObject.transform.Rotate (Vector3.up * Time.deltaTime * 60);
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;
		string objTag = obj.tag;

		if (objTag.Equals ("Player")) {
			switch (this.type) {
			case Powerup_Controller.POWERUP_TYPE.BOMB:
				OnBombPowerUp (powerupDuration);
				break;
			case Powerup_Controller.POWERUP_TYPE.HAY:
				OnHayPowerUp (powerupDuration);
				Instantiate (tempHayPrefab, this.transform.position, Quaternion.identity);
				break;
			case Powerup_Controller.POWERUP_TYPE.TRACTOR:
				OnTractorPowerUp (powerupDuration);
				break;
			default:
				break;
			}

			Destroy (this.gameObject);
		}
	}
}
