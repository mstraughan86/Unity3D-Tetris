using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	// Groups
	public GameObject[] groups;

	// Use this for initialization
	void Start () {
		SpawnNext();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// STRETCH GOAL *****
	// Create the 7 Block Bag randomizer!
	// on round start-up, populate a list of 128 queued blocks

	public void SpawnNext() {
		// Random Index
		int i = Random.Range(0, groups.Length);

		// Spawn Group at current Position
		Instantiate(groups[i], transform.position, Quaternion.identity);
	}

	//public static void 
}
