using UnityEngine.SceneManagement;
using UnityEngine;

public class BackButtonHandling : MonoBehaviour 
{
    float timeSinceClicked = 0;
    int comboClickCount = 0;

    void Update () {
        timeSinceClicked += Time.deltaTime;

        if (timeSinceClicked > 1)
        {
            if(comboClickCount > 0)
                comboClickCount -= 1;
            timeSinceClicked = 0;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            comboClickCount += 1;
            timeSinceClicked = 0;
            if (comboClickCount >= 4)
                SceneManager.LoadScene("Menu");
        }
            
	}
}
