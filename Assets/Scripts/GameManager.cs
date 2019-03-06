using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To manipulate the slider which denotes our speed.

public class GameManager : MonoBehaviour {

	public static GameManager instance; // Singleton design pattern

	public float speed = 1f; // The speed of the player. // NOT USED HERE

	public const float MIN_SPEED = 0f; // USED ELSEWHERE
	public const float MAX_SPEED = 2f;

    public const float TILT_THRESHHOLD = 0.5f; // USED IN PLAYERMOVEMENT

	//private const float MIN_Z_TILT = -0.25f;
	//private const float MAX_Z_TILT = -1f;

	//private bool isPhone = false; // REPLACED WITH PREPROCESSOR

    //the values of the acceleration to be exposed
    public float Xinput;
    public float Yinput;
    public float Zinput;

    private Queue<float> XValues; //queue that hold the past few frames of data of the input
    private Queue<float> YValues;
    private Queue<float> ZValues;

    private int inputBufferFrames = 10; // number of frames the input will average for the value

	private bool gameIsOver = false;
	private float time = 30f; // Time remaining for the player.

	//public Slider speedometer;
	//public Text test;

	// Set up the singleton design pattern
	private void Awake() {
		if (instance == null) {
			instance = this;//setup singleton
		}
	}
    public float GetTime() { return time; }
    public void SetTime(float t) { time = t;}
	public void GameOver() {
		gameIsOver = true;
		Jukebox.instance.CallDamaged();
		UIManager.instance.ShowGameOverPanel();
	}

	private void Start() {

        XValues = new Queue<float>();
        YValues = new Queue<float>();
        ZValues = new Queue<float>();

        //fill the queues with 0.0fs, these values get replaced quickly but are important so the queue is at the desired size
        for(int i = 0; i < inputBufferFrames; i++)
        {
            XValues.Enqueue(0.0f);
            YValues.Enqueue(0.0f);
            ZValues.Enqueue(0.0f);
        }
        Debug.Log(XValues);
	}

	public bool GetGameIsOver() {
		return gameIsOver;
	}

	private void Update() {
        Xinput = UpdateXInput();
        //Yinput = UpdateYInput(); //not needed anywhere
        Zinput = UpdateZInput();
#if UNITY_EDITOR // this is pointless for now but im going to leave it in just in case

#else //this would be on a phone
        //call the UI Manager to update the values
            UIManager.instance.UpdateUI(); //update the UI to match the tilt if on a phone
#endif
		time -= Time.deltaTime;
        //checking if checkpoint reached
        //if(checkpoint is reached)
        //timer+=Time.deltaTime*10;
        if (time <= 0 || gameIsOver) {
			UIManager.instance.timer.text = "OVER";

			if (!gameIsOver) {
				GameOver();
			}
		}
        else {
			UIManager.instance.timer.text = Mathf.Round(time).ToString();
		}
    }

    //helper function to help unclutter the main update loop
    private float UpdateXInput()//it will always add one to the queue then take one away, keeping the number the same
    {
#if UNITY_EDITOR
        XValues.Enqueue(Mathf.Round(UIManager.instance.tiltometer.value * 100.0f) / 100.0f);
#else //this would be on a phone
        XValues.Enqueue(Mathf.Round(-Input.acceleration.x * 100.0f) / 100.0f);
#endif
        XValues.Dequeue();
        return AverageQueueData(XValues); //average the queue values to smooth the input
    }
    //helper function to help unclutter the main update loop
    private float UpdateYInput() //completely unused but we have it in case
    {
#if UNITY_EDITOR
        YValues.Enqueue(0.0f);
#else //this would be on a phone
        YValues.Enqueue(0.0f);
#endif
        YValues.Dequeue();
        return AverageQueueData(YValues); //average the queue values to smooth the input
    }

    //helper function to help unclutter the main update loop
    private float UpdateZInput()
    {
#if UNITY_EDITOR
        ZValues.Enqueue(Mathf.Round(UIManager.instance.speedometer.value * 100.0f) / 100.0f);
#else //this would be on a phone
        ZValues.Enqueue((Mathf.Round(-Input.acceleration.z * 100.0f) / 100.0f)-0.25f);//take out the -0.25f if it is causing too much of an issue
        // the goal is to offset the input so the phone doesnt have to be held straight up
        //alternatively just change it to a + if it tilts it the wrong way
#endif
        ZValues.Dequeue();
        return AverageQueueData(ZValues); //average the queue values to smooth the input
    }

    //returns the average value of the input queue
    private float AverageQueueData(Queue<float> data)
    {
        float avg = 0.0f;
        foreach(float f in data)
        {
            avg += f;
        }
        avg /= inputBufferFrames;
        return Mathf.Round(avg * 100.0f) / 100.0f;
    }
}
