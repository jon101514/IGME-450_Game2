using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public int leanState; //I used an int here because a bool couldn't store the 3 states and I didnt want to use more than one variable for this

	public const float MIN_SPEED = 1/16f; 
	public const float MAX_SPEED = 16f;

	private const float MIN_Z_TILT = -0.25f;
	private const float MAX_Z_TILT = -1f;

	private const float LEAN_TIME = 0.5f; // Time it takes to lean.
	private bool isLeaning = false;

	private Transform tm; // Shorthand for "transform"

	private void Awake() {
		tm = GetComponent<Transform>();
	}

    private void Update () {
        UpdateSpeed(); // this sets the speed variable in the method
        leanState = UpdateLean(); //we will need a method that takes in lean state to do the visual rotation of the cart
#if UNITY_EDITOR
        Debug.Log("Player Speed: " + speed + "\nPlayer Lean: " + leanState);
#endif
    }

	// FIX
    //scales the z input from the gamemanager to a reasonable speed
    private float UpdateSpeed()
    {
        //do any modifications to the speed value under this line but before assigning the gamemanager speed value
        speed = GameManager.instance.Zinput * 2;
        //any modifications from the raw input to spped go here in the form of speed = newSpeed


        
		tm.Translate(Vector3.forward * speed);
		GameManager.instance.speed = speed;

        return speed; 
    }

        //gets the lean values from game manager, returns -1 for left, 0 for center, and 1 for right
        private int UpdateLean()
    {
        if (GameManager.instance.Xinput <= GameManager.TILT_THRESHHOLD * -0.5)
        {			
			// if (!isLeaning) {StartCoroutine(LeanAnim());}
            return -1;
        } else if (GameManager.instance.Xinput >= GameManager.TILT_THRESHHOLD * 0.5)
        {
			// if (!isLeaning) {StartCoroutine(LeanAnim());}
            return 1;
        } else
        {
			// isLeaning = false;
            return 0;
        }
    }

	// Lerps the cart's rotation visually.
	private IEnumerator LeanAnim() {
		isLeaning = true;
		float timer = 0f;
		while (timer <= LEAN_TIME) {
			Debug.Log("timer / lean time" + timer / LEAN_TIME);
			tm.rotation *= new Quaternion(0, 0, leanState / 45f, tm.rotation.w);
			yield return new WaitForSecondsRealtime(Time.deltaTime);
			timer += Time.deltaTime;
		}
	}
		
	// From https://stackoverflow.com/questions/3451553/value-remapping
	private float Map(float value, float low1, float high1, float low2, float high2) {
		return low2 + (high2 - low2) * ((value - low1) / (high1 - low1));
	}
}
