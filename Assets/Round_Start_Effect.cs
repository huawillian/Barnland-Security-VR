using UnityEngine;
using System.Collections;

public class Round_Start_Effect : MonoBehaviour
{
	ParticleSystem particles;
	float effectDuration = 2.0f;

	// Use this for initialization
	void Start () {
		particles = this.GetComponent<ParticleSystem> ();
		particles.Stop ();
	}

	void OnEnable()
	{
		Round_Manager.OnRoundOver += startEffect;
	}

	void OnDisable()
	{
		Round_Manager.OnRoundOver -= startEffect;
	}

	void startEffect()
	{
		StartCoroutine ("roundStartEffectCoroutine");
	}

	IEnumerator roundStartEffectCoroutine()
	{
		particles.Play ();
		yield return new WaitForSeconds (effectDuration);
		particles.Stop ();
	}


	// Update is called once per frame
	void Update () {
	
	}
}
