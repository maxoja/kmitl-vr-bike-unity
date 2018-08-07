using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCam : MonoBehaviour {

    public float sensitivity = 4;
	
    void Update () {
        float changeX = Input.GetAxis("Mouse X");
        float changeY = Input.GetAxis("Mouse Y");

        if(Input.touchCount > 0)
        {
            changeX = Input.touches[0].deltaPosition.x/10f;
            changeY = Input.touches[0].deltaPosition.y/10f;
        }

        for (int i = 0; i < 4; i++)
        {
            transform.Rotate(-changeY, 0, 0, Space.Self);
            transform.parent.Rotate(0,changeX,0, Space.Self);
        }
	}
}
