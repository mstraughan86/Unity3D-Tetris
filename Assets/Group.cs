using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {

	// Time since last gravity tick
	float lastFall;
	static float fallRate = 1;

	// Use this for initialization
	void Start () {
		lastFall = 0;
		Debug.Log ("Spawning new Block Group");

		// Default position not valid? Then it's game over
		if (!IsValidGridPos ()) {
			Debug.Log ("Game Over");
			Destroy (gameObject);
		}
	}

	void Update () {
		if (Time.time - lastFall >= fallRate) {
			Fall ();
		}

		// STRETCH GOAL *****
		// Recreate the control sceme so that there is:
		// Arrow Config, WASD Config, or User Selectable Controls
		// This requires that the control logic be value agnostic, relying on the variable only
		// NOTE: Possibly remove controls from the individual block groups and put it elsewhere

		// Using string as a quick and easy fix that corrects a few control problems:
		// 1. Hold Key Press, 2. Remove If-Else Run-On, 3. Enable switch statement
		string keyboardInputString = Input.inputString;
		switch (keyboardInputString) {
		case "w":
			RotateLeft ();
			break;
		case "a":
			MoveLeft ();
			break;
		case "s":
			Fall ();
			break;
		case "d":
			MoveRight ();
			break;
		case " ":
			Drop ();
			break;
		}
	}

	bool IsValidGridPos() {
		foreach (Transform child in transform) {
			Vector2 v = Grid.RoundVec2 (child.position);

			// Not inside Border?
			if (!Grid.InsideBorder (v)) {
				return false;
			}

			// Block in grid cell (and not part of same gruop)?
			if (Grid.grid [(int)v.x, (int)v.y] != null && Grid.grid [(int)v.x, (int)v.y].parent != transform) {
				return false;
			}

		}
		return true;
	}

	void UpdateGrid() {
		// Remove old children from grid
		for (int y = 0; y < Grid.h; ++y) {
			for (int x = 0; x < Grid.w; ++x) {
				if (Grid.grid [x, y] != null) {
					if (Grid.grid [x, y].parent == transform) {
						Grid.grid [x, y] = null;
					}
				}
			}
		}

		// Add new children to grid
		foreach (Transform child in transform) {
			Vector2 v = Grid.RoundVec2 (child.position);
			Grid.grid [(int)v.x, (int)v.y] = child;
		}

	}

	private void BlockFallRecursion() {
		// Check if last position transform was valid
		if (!IsValidGridPos ()) {

			// FAIL: revert up one position and terminate recursion
			transform.position += new Vector3 (0, 1, 0);
			return;
		} else {

			// PASS: update the grid for the previous move position
			UpdateGrid();
		}

		// Recursive Modify Position
		transform.position += new Vector3 (0, -1, 0);

		// Recursion
		BlockFallRecursion();
	}

	private void MoveLeft() {
		transform.position += new Vector3 (-1, 0, 0);

		if (IsValidGridPos ())
			UpdateGrid ();
		else
			transform.position += new Vector3 (1, 0, 0);
	}

	private void MoveRight() {
		transform.position += new Vector3 (1, 0, 0);

		if (IsValidGridPos ())
			UpdateGrid ();
		else
			transform.position += new Vector3 (-1, 0, 0);
	}

	private void RotateLeft() {
		transform.Rotate (0, 0, 90);

		if (IsValidGridPos ())
			UpdateGrid ();
		else
			transform.Rotate (0, 0, -90);
	}

	private void RotateRight() {
		transform.Rotate (0, 0, -90);

		if (IsValidGridPos ())
			UpdateGrid ();
		else
			transform.Rotate (0, 0, 90);
	}

	private void Fall() {
		transform.position += new Vector3(0, -1, 0);

		if (IsValidGridPos ()) {
			UpdateGrid ();
		}
		else {
			transform.position += new Vector3 (0, 1, 0);

			CleanUp ();
		}
		lastFall = Time.time;
	}

	private void Drop() {
		// Initial modify position
		transform.position += new Vector3 (0, -1, 0);

		// Call recursion to force block to bottom most valid position
		BlockFallRecursion ();

		CleanUp ();
	}

	private void CleanUp() {
		// Clear filled horizontal lines
		Grid.DeleteFullRows ();

		// Spawn Next Group
		FindObjectOfType<Spawner>().SpawnNext();

		// Disable script
		enabled = false;
	}
}