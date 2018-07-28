using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IndependentParent : MonoBehaviour 
{
    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;

    public float rotX = 0;
    public float rotY = 0;
    public float rotZ = 0;

	void Update () {
        if (posX == 0 && posY == 0 && posZ == 0)
            if (rotX == 0 && rotY == 0 && rotZ == 0)
                return;
                
        List<Transform> children = new List<Transform>();
        while ( transform.childCount > 0 )
        {
            Transform child = transform.GetChild(0);
            children.Add(child);
            child.parent = null;
        }

        transform.Translate(posX, posY, posZ, Space.Self);
        transform.Rotate(rotX, rotY, rotZ, Space.Self);
        posX = 0;
        posY = 0;
        posZ = 0;
        rotX = 0;
        rotY = 0;
        rotZ = 0;

        foreach (Transform child in children)
            child.parent = transform;
	}
}
