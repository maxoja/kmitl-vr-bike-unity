using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendPoint : MonoBehaviour 
{
    public Vector3 position { get { return this.transform.position; } }
    public Quaternion rotation { get { return this.transform.rotation; } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(position, 1);
    }
}
