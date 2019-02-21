using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public int leanState; //I used an int here because a bool couldn't store the 3 states and I didnt want to use more than one variable for this

	private const float LEAN_TIME = 0.5f; // Time it takes to lean.
	private bool isLeaning = false;

	private Transform tm; // Shorthand for "transform"

	private void Awake() {
		tm = GetComponent<Transform>();
	}

    private void Update () {
        speed = UpdateSpeed();
        leanState = UpdateLean(); //we will need a method that takes in lean state to do the visual rotation of the cart
#if UNITY_EDITOR
        Debug.Log("Player Speed: " + speed + "\nPlayer Lean: " + leanState);
#endif
    }

    //scales the z input from the gamemanager to a reasonable speed
    private float UpdateSpeed()
    {
        //map the value, apparently set up so mapping is uneccessary
		tm.Translate(Vector3.forward * speed);
        return GameManager.instance.Zinput;
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
		
}
