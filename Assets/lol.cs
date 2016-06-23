using UnityEngine;
using System.Collections;


public class lol : MonoBehaviour {
	IFirebase root;
	IDataSnapshot shot;

	void Awake() {
		root = Firebase.CreateNew("https://barnland-security-vr-test.firebaseio.com/Leaderboard/");
	}

	void OnEnable() {
		root.ValueUpdated += FooChildAdded;
	}

	void OnDisable() {
	}

	void FooChildAdded (object sender, FirebaseChangedEventArgs e) {
		Debug.Log("ValueUpdated: " + e.DataSnapshot.StringValue);
		shot = e.DataSnapshot;

		Debug.Log (shot.Child("Testing").StringValue);

		root.ValueUpdated -= FooChildAdded;
	}
}
