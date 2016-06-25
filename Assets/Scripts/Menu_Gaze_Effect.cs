using UnityEngine;
using System.Collections;

public class Menu_Gaze_Effect : MonoBehaviour
{
	public GameObject particleObj;
	private ParticleSystem particles;

	public GameObject menuObj;

	// Use this for initialization
	void Start () {
		particles = particleObj.GetComponent<ParticleSystem> ();
		particles.Stop ();
	}

	public void OnGazeEnter()
	{
		menuObj.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
		particles.Play ();
	}

	public void OnGazeExit()
	{
		menuObj.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		particles.Stop ();
	}

	void OnEnable()
	{
		particles = particleObj.GetComponent<ParticleSystem> ();
		particles.Stop ();
		menuObj.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}

}
