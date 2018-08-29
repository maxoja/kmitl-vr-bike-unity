using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SpeedCam : MonoBehaviour 
{
    private static float normalFieldOfView = 100;
    private static float fastFieldOfView = 90;
    private static float normalSpeed = 0;
    private static float fastSpeed = 0.5f;
    private Camera cam;
    private Vector3 prevPos;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        prevPos = transform.position;
    }
    private void Update()
    {
        float deltaDist = Vector3.Distance(prevPos, transform.position);
        float ratio = Mathf.Lerp(0, 1, deltaDist / (fastSpeed - normalSpeed));
        cam.fieldOfView = Mathf.Lerp(normalFieldOfView, fastFieldOfView, ratio);
        prevPos = transform.position;
    }

}
