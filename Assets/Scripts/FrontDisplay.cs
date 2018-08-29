using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontDisplay : MonoBehaviour {
    public BezierWalker walker;
    public Text textComp;
    public PlayerSync sync;

    float t = 0;
	void Update () {
        t += Time.deltaTime;
        if(t >= 6)
        {
            t = 0;
        }

        if (t <= 3)
            textComp.text = "Player " + (1 + sync.id).ToString() + "\n" + walker.GetProgress().ToString("P1");
        else
            textComp.text = "Player " + (1 + sync.id).ToString() + "\n" + (walker.GetProgress() * 18).ToString("F1") + " cal";
	}
}
