/** Calculates the score and tells the UI Manager to display the update to the player.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	public static ScoreManager instance; // Singleton design pattern
	[SerializeField]
	private int score = 0;

	private const int BASE_SCORE = 100; // Give the player this amount of points for every interval they survive.
	private const float INTERVAL = 1/10f; // Calculate the score every XX seconds.
	private const float EPSILON = 0.005f;

	private int[] VR_SCORES = {25000, 50000, 100000, 150000}; // When the player exceeds these scores, add in a vertically-remixed music layer.
	private int vrScoresIndex = 0; // Current index we're checking for in VR_SCORES.

	// Getter for the multiplier value.
	public float GetMultiplier() {
		// If the speed is (roughly) zero, don't add any points.
		if (GameManager.instance.speed <= EPSILON) {
			return 0;
		}
		// Otherwise, add to the score as a function of speed.
		return (1f + GameManager.instance.speed);
	}

	// Getter for the score.
	public int GetScore() {
		return score;
	}

	// Set up the singleton design pattern.
	private void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	// Call the function which calculates our score on a repeating interval.
	private void Start() {
		InvokeRepeating("CalculateScore", 0f, INTERVAL);
	}

	private void CalculateScore() {
		// If we've gotten a game over, stop adding to the score.
		if (GameManager.instance.GetGameIsOver()) {
			CancelInvoke("CalculateScore");
			return;
		}
		VerticalRemixLayers();
		score += (int) (BASE_SCORE * GetMultiplier());
	}

	// At certain score thresholds, add a new track of music. This checks to see if the player has reached those thresholds.
	private void VerticalRemixLayers() {
		if (vrScoresIndex >= VR_SCORES.Length) {
			return;
		}
		if (score >= VR_SCORES[vrScoresIndex]) {
			if (vrScoresIndex == 0) {
				Debug.Log("Add Simple Drums");
				Jukebox.instance.AddSpeaker(2);
			} else if (vrScoresIndex == 1) {
				Debug.Log("Add Backup");
				Jukebox.instance.AddSpeaker(3);
			} else if (vrScoresIndex == 2) {
				Debug.Log("Add Advanced Drums");
				Jukebox.instance.AddSpeaker(4);
			} else if (vrScoresIndex == 3) {
				Debug.Log("Add Extra");
				Jukebox.instance.AddSpeaker(5);
			}

			vrScoresIndex++;
		}
	}

}
