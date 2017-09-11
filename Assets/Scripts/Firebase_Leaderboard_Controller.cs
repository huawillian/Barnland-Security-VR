using UnityEngine;
using System.Collections;

public class Firebase_Leaderboard_Controller : MonoBehaviour 
{
	IFirebase root;
	IDataSnapshot shot;
	//public string url = "https://barnland-security-vr-test.firebaseio.com/Leaderboard/";

	public bool dataRetrievalInProcess = false;
	public float timeOutDuration = 5.0f;

	// Events
	public delegate void FirebaseEvents(LeaderboardEntry[] leaderboardEntries);
	public static event FirebaseEvents OnDataReceived;
	public static event FirebaseEvents OnDataTimeout;

	public struct LeaderboardEntry
	{
		public string name;
		public int score;
	}

	LeaderboardEntry[] leaderboardEntries = new LeaderboardEntry[10];

	// Use this for initialization
	void Start () {
		//root = Firebase.CreateNew("https://barnland-security-vr-test.firebaseio.com/Leaderboard/");
	}

	public void resetLeaderboardOnFirebase()
	{
		for (int i = 1; i <= 10; i++) {
			IFirebase entry = root.Child (i.ToString ());
			entry.Child ("Name").SetValue("AAAAAAAAAA");
			entry.Child ("Score").SetValue ("0");
		}
	}

	public void retrieveLeaderboardOnFirebase()
	{
		//root.ValueUpdated += OnReceiveFirebaseData;
		//dataRetrievalInProcess = true;
		//StartCoroutine ("dataRetrievalTimeoutCoroutine");
	}

	void OnReceiveFirebaseData (object sender, FirebaseChangedEventArgs e) {
		shot = e.DataSnapshot;
		Debug.Log("ValueUpdated: " + shot.StringValue);
		root.ValueUpdated -= OnReceiveFirebaseData;
		dataRetrievalInProcess = false;
		dataSnapshotToleaderboardEntries ();
		OnDataReceived (leaderboardEntries);
	}

	private IEnumerator dataRetrievalTimeoutCoroutine()
	{
		yield return new WaitForSeconds (timeOutDuration);
		if (dataRetrievalInProcess) {
			dataRetrievalInProcess = false;
			root.ValueUpdated -= OnReceiveFirebaseData;
			Debug.Log ("Data Retrieval from Firebase has timed out...");
			OnDataTimeout (null);
		}
	}

	private void dataSnapshotToleaderboardEntries()
	{
		for (int i = 1; i <= 10; i++) {
			IDataSnapshot snapshot = shot.Child (i.ToString());
			leaderboardEntries [i - 1].name = snapshot.Child ("Name").StringValue;
			leaderboardEntries [i - 1].score = int.Parse(snapshot.Child ("Score").StringValue);
		}
	}

	public void leaderboardEntriesToFirebase(LeaderboardEntry[] entriesTemp)
	{
		for (int i = 0; i < 9; i++) {
			IFirebase entry = root.Child ((i + 1).ToString ());
			entry.Child ("Name").SetValue(entriesTemp[i].name);
			entry.Child ("Score").SetValue (entriesTemp[i].score.ToString());

			leaderboardEntries [i].name = entriesTemp [i].name;
			leaderboardEntries [i].score = entriesTemp [i].score;
		}
	}

}
