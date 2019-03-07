using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour {

    private Camera cam;

    private Vector3 basePosition;
    private float baseFOV;

    public float cameraMove = 1.5f;
    public float FOVMove = 50;

	// Use this for initialization
	void Start () {
        cam = gameObject.GetComponent<Camera>();
        basePosition = gameObject.transform.localPosition;
        baseFOV = cam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
        if(GameManager.instance.speed > 0.65f) //0.65 is just an arbitrary value i picked to trigger the speed effects
        {
            transform.localPosition = new Vector3(basePosition.x, basePosition.y, basePosition.z + (cameraMove * (0.65f - GameManager.instance.speed)));
            cam.fieldOfView = baseFOV + (FOVMove * GameManager.instance.speed);
        }
        else
        {
            transform.localPosition = new Vector3(basePosition.x, basePosition.y, basePosition.z);
            cam.fieldOfView = baseFOV;
        }     
           
    }
}
