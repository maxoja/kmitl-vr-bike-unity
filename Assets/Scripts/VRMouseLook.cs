using UnityEngine;

public class VRMouseLook : MonoBehaviour
{
    public bool force = false;

    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;

    public bool enableYaw = true;
    public bool autoRecenterPitch = true;
    public bool autoRecenterRoll = true;

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        bool rolled = false;
        bool pitched = false;
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
            pitched = true;
            if (enableYaw) {
                mouseX += Input.GetAxis("Mouse X") * 5;
                if (mouseX <= -180) {
                    mouseX += 360;
                } else if (mouseX > 180) {
                    mouseX -= 360;
                }
            }
            mouseY -= Input.GetAxis("Mouse Y") * 2.4f;
            mouseY = Mathf.Clamp(mouseY, -85, 85);
        } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            rolled = true;
            mouseZ += Input.GetAxis("Mouse X") * 5;
            mouseZ = Mathf.Clamp(mouseZ, -85, 85);
        }
        if (!rolled && autoRecenterRoll) {
            // People don't usually leave their heads tilted to one side for long.
            mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }
        if (!pitched && autoRecenterPitch) {
            // People don't usually leave their heads tilted to one side for long.
            mouseY = Mathf.Lerp(mouseY, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }
        transform.localRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);
#else
        if(force)
        {
        bool rolled = false;
            bool pitched = false;
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                pitched = true;
                if (enableYaw)
                {
                    mouseX += Input.GetAxis("Mouse X") * 5;
                    if (mouseX <= -180)
                    {
                        mouseX += 360;
                    }
                    else if (mouseX > 180)
                    {
                        mouseX -= 360;
                    }
                }
                mouseY -= Input.GetAxis("Mouse Y") * 2.4f;
                mouseY = Mathf.Clamp(mouseY, -85, 85);
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                rolled = true;
                mouseZ += Input.GetAxis("Mouse X") * 5;
                mouseZ = Mathf.Clamp(mouseZ, -85, 85);
            }
            if (!rolled && autoRecenterRoll)
            {
                // People don't usually leave their heads tilted to one side for long.
                mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
            }
            if (!pitched && autoRecenterPitch)
            {
                // People don't usually leave their heads tilted to one side for long.
                mouseY = Mathf.Lerp(mouseY, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
            }
            transform.localRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);
        }
#endif
    }



}