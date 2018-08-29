using UnityEngine;
using UnityEngine.SceneManagement;

public class MonitorByPass : MonoBehaviour 
{    
	void Update () 
    {
        if(Input.GetKey("m"))
        {
            SceneManager.LoadScene("Monitor");
        }
	}
}
