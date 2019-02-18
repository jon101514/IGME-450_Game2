using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To manipulate the slider which denotes our speed.

public class GameManager : MonoBehaviour {

	public static GameManager instance; // Singleton design pattern

	public float speed = 1f; // The speed of the player.

	private const float MIN_SPEED = 1/16f; 
	private const float MAX_SPEED = 16f;  

	private const float MIN_Z_TILT = -0.25f;
	private const float MAX_Z_TILT = -1f;

	private bool isPhone = false;

	public Slider speedometer;
	public Text test;

	// Set up the singleton design pattern
	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		// Set up the speedometer.
		speedometer.minValue = MIN_SPEED;
		speedometer.maxValue = MAX_SPEED;
	}

	private void Start() {
		if (Input.acceleration == Vector3.zero) {
			Debug.Log("No phone detected");
			isPhone = false;
		} else {
			isPhone = true;
		}
	}

	private void Update() {
		// Input.acceleration should help us here, but we're going to tie our speed to Input.acceleration and then update the UI's slider.
		test.text = "ISPHONE: " + isPhone + "\nZ:" + Input.acceleration.z + "\nSPEED2: " + Map(-Input.acceleration.z, -MIN_Z_TILT, -MAX_Z_TILT, MIN_SPEED, MAX_SPEED) + "\nSPEEDOMETER: " + speed;
		// Calculate speed based on acceleration.
		if (isPhone && speed >= MIN_SPEED) {
			speed = Map(-Input.acceleration.z, -MIN_Z_TILT, -MAX_Z_TILT, MIN_SPEED, MAX_SPEED);
		} else if (isPhone) {
			speed = MIN_SPEED;
		} else {
			speed = speedometer.value;
		}
		if (isPhone) {
			UpdateUI();
		}
	}

	// Update the UI slider.
	private void UpdateUI() {
		speedometer.value = speed;	
	}

	// From https://stackoverflow.com/questions/3451553/value-remapping
	private float Map(float value, float low1, float high1, float low2, float high2) {
		return low2 + (high2 - low2) * ((value - low1) / (high1 - low1));
	}
}
