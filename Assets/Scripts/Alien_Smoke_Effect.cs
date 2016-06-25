using UnityEngine;
using System.Collections;

public class Alien_Smoke_Effect : MonoBehaviour
{
	public GameObject smokeObj;
	private ParticleSystem smoke;
	private float duration;

	// Use this for initialization
	void Start ()
	{
		smoke = smokeObj.GetComponent<ParticleSystem> ();
		smoke.Stop ();
	}

	void OnEnable()
	{
		Powerup_Controller.OnBombPowerUp += startSmokeCoroutine;
	}

	void OnDisable()
	{
		this.StopAllCoroutines ();
		Powerup_Controller.OnBombPowerUp -= startSmokeCoroutine;
	}


	void startSmokeCoroutine(float dur)
	{
		duration = dur;
		StartCoroutine ("smokeCoroutine");
	}

	IEnumerator smokeCoroutine()
	{
		smoke.Play ();
		yield return new WaitForSeconds (duration);
		smoke.Stop ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
