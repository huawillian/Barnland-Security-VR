using UnityEngine;
using System.Collections;

public class Animal_Controller : MonoBehaviour
{
	//type of animal
	public enum ANIMAL_TYPE{COW, PIG, CHICKEN};
	public ANIMAL_TYPE type;

	//state
	public enum ANIMAL_STATE{MOVING, DISABLED};
	public ANIMAL_STATE state = ANIMAL_STATE.MOVING;

	//navagent
	public NavMeshAgent agent;
	public bool destinationSet = false;
	public float timeSet = 0.0f;
	public float moveTimeInterval = 5.0f;

	//alien capturing this animal
	public GameObject alien = null;

	// Use this for initialization
	void Start ()
	{
		agent = this.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (state == ANIMAL_STATE.MOVING) {
			if (!destinationSet) {
				destinationSet = true;
				Vector3 currPos = this.transform.position;
				Vector3 newPos = new Vector3 (currPos.x + Random.Range(-15,15), 
					currPos.y, 
					currPos.z + Random.Range(-15,15));
				agent.destination = newPos;
				timeSet = Time.time;
			} else if (Time.time > timeSet + moveTimeInterval) {
				// time up
				destinationSet = false;
			} else if ((this.transform.position - agent.destination).magnitude < 1.0f) {
				// Animal has reached destination before time
				destinationSet = false;
			} else {
				// Animal is moving to destination
			}
		}
	}

	public void disableAnimal()
	{
		this.state = ANIMAL_STATE.DISABLED;
		this.agent.enabled = false;
	}

	public void setDestinationtoPlayer(Vector3 playerLocation)
	{
		if (state != ANIMAL_STATE.DISABLED) {
			destinationSet = true;
			agent.destination = playerLocation;
			timeSet = Time.time;
		}
	}
}
