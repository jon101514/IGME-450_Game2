using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To manipulate the slider which denotes our speed.

public class GameManager : MonoBehaviour {

	public static GameManager instance; // Singleton design pattern

	public float speed = 1f; // The speed of the player.

	private const float MIN_SPEED = 1/16f; // Clamp to -0.25f
	private const float MAX_SPEED = 16f;  // Clamp to -1f

	public Text test;

	// Set up the singleton design pattern
	private void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	private void Update() {
		// Input.acceleration should help us here, but we're going to tie our speed to Input.acceleration and then update the UI's slider.
		test.text = "Z:" + Input.acceleration.z;
	}
}
