using UnityEngine;
using System.Collections;

public class Round_Data : MonoBehaviour
{
	public struct ROUND_VARIABLES {
		public int roundNumber;
		public int animalsNeeded;
		public int animalsSpawned;
		public int aliensSpawned;
		public float roundDuration;
	}

	public ROUND_VARIABLES[] roundData = new ROUND_VARIABLES[15];

	void Awake()
	{
		int alienCounter = 0;

		for (int i = 0; i < 15; i++) {
			roundData [i].roundNumber = i + 1;
			roundData [i].animalsNeeded = i + 3;
			roundData [i].animalsSpawned = (i + 3) * 2;
			roundData [i].roundDuration = 90;
			if (i == 1) {
				alienCounter++;
			}

			if (i % 5 == 0 && i != 0) {
				alienCounter++;
			}

			roundData [i].aliensSpawned = alienCounter;
		}
	}
}
