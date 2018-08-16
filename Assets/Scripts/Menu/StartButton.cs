using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
    //public Scene scene;

    public void OnClick()
    {
        SceneManager.LoadScene("Game");

    }
}
