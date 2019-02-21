using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To manipulate the slider which denotes our speed.

public class UIManager : MonoBehaviour
{

    public static UIManager instance; // Singleton design pattern

    public Slider speedometer; // the forward back tilting, tracks the z axis
    public Slider tiltometer; //the left right tilting, tracks the x axis
    public Text test;

    // Set up the singleton design pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // Set up the speedometer
        speedometer.minValue = GameManager.MIN_SPEED;
        speedometer.maxValue = GameManager.MAX_SPEED;

        //Setup the tiltometer
        tiltometer.minValue = -GameManager.TILT_THRESHHOLD;
        tiltometer.maxValue = GameManager.TILT_THRESHHOLD;
    }

    private void Start()
    {
      
    }

    private void Update()
    {
#if UNITY_EDITOR
        test.text = "In editor\nX: " + GameManager.instance.Xinput + "\nY: " + GameManager.instance.Yinput + "\nZ: " + GameManager.instance.Zinput;
#else //this would be on a phone
        test.text = "In phone \nX: " + GameManager.instance.Xinput + "\nY: " + GameManager.instance.Yinput + "\nZ: " + GameManager.instance.Zinput;
        UpdateUI();
#endif
		Jukebox.instance.ChangeVolume(0, speedometer.normalizedValue);
    }

    // Update the UI slider.
    public void UpdateUI()
    {
        speedometer.value = GameManager.instance.Zinput;
        tiltometer.value = GameManager.instance.Xinput;
    }
}
