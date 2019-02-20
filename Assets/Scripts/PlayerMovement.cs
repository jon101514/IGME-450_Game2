using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public int leanState; //I used an int here because a bool couldn't store the 3 states and I didnt want to use more than one variable for this

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
        return GameManager.instance.Zinput;
    }

        //gets the lean values from game manager, returns -1 for left, 0 for center, and 1 for right
        private int UpdateLean()
    {
        if (GameManager.instance.Xinput <= GameManager.TILT_THRESHHOLD * -0.5)
        {
            return -1;
        } else if (GameManager.instance.Xinput >= GameManager.TILT_THRESHHOLD * 0.5)
        {
            return 1;
        } else
        {
            return 0;
        }
    }
}
