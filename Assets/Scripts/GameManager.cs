using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To manipulate the slider which denotes our speed.

public class GameManager : MonoBehaviour {

	public static GameManager instance; // Singleton design pattern

	public float speed = 1f; // The speed of the player.

	public const float MIN_SPEED = 0f; 
	public const float MAX_SPEED = 2f;

    public const float TILT_THRESHHOLD = 0.5f;

	//private const float MIN_Z_TILT = -0.25f;
	//private const float MAX_Z_TILT = -1f;

	private bool isPhone = false;

    //the values of the acceleration to be exposed
    public float Xinput;
    public float Yinput;
    public float Zinput;

	private bool gameIsOver = false;
	private float time = 90f; // Time remaining for the player.

	//public Slider speedometer;
	//public Text test;

	// Set up the singleton design pattern
	private void Awake() {
		if (instance == null) {
			instance = this;//setup singleton
		}
	}

	public void GameOver() {
		gameIsOver = true;
		Jukebox.instance.CallDamaged();
	}

	private void Start() {
		StartCoroutine(VerticalRemixDemo());
	}

	// After 15 seconds, add a new musical layer. This is because we don't know how gameplay will affect music as feedback, so this just demonstrates it.
	private IEnumerator VerticalRemixDemo() {
		yield return new WaitForSecondsRealtime(15f);
		Debug.Log("Add Backup and Drums");
		Jukebox.instance.AddSpeaker(2);
		yield return new WaitForSecondsRealtime(15f);
		Debug.Log("Add Extra Layers");
		Jukebox.instance.AddSpeaker(3);
	}

	private void Update() {
#if UNITY_EDITOR
        //get the inputs directly from the UI
        Xinput = Mathf.Round(UIManager.instance.tiltometer.value * 100.0f) / 100.0f;
        Yinput = 0.0f;
        Zinput = Mathf.Round(UIManager.instance.speedometer.value * 100.0f) / 100.0f;
#else //this would be on a phone
        //get all 3 axis of rotation, rounded to two decimal places
            Xinput = Mathf.Round(-Input.acceleration.x * 100.0f)/100.0f;
            Yinput = Mathf.Round(-Input.acceleration.y * 100.0f)/100.0f;
            Zinput = Mathf.Round(-Input.acceleration.z * 100.0f)/100.0f;
        //call the UI Manager to update the values
            UIManager.instance.UpdateUI();
#endif
		time -= Time.deltaTime;
		if (time <= 0 || gameIsOver) {
			UIManager.instance.timer.text = "OVER";

			if (!gameIsOver) {
				gameIsOver = true;
				Jukebox.instance.CallDamaged();
			}
		} else {
			UIManager.instance.timer.text = Mathf.Round(time).ToString();
		}
    }
		
}
