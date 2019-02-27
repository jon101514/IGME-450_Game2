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
		score += (int) (BASE_SCORE * GetMultiplier());
	}

}
