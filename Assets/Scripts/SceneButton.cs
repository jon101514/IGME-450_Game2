/** Jonathan So, jds7523@rit.edu
 * Handles the UI button's scene changes.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour {

	public void StartGame() {
		PlayerPrefs.SetInt("Day", 1); // Initialize the data to Day 1.
		SceneManager.LoadScene("Calendar");
	}

	public void ChangeScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public void QuitGame() {
		Application.Quit();
	}
}
