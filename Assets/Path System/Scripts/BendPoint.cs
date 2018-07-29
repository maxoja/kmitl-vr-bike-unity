using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendPoint : MonoBehaviour 
{
    public Vector3 position { get { return this.transform.position; } }
    public Quaternion rotation { get { return this.transform.rotation; } }
    //todo draw arrow for bend points

    private const float gizmoLength = 4;
    private const float gizmoWidth = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(position, 1);

        Vector3 front = position + transform.forward * gizmoLength;
        Vector3 right = position + transform.right * gizmoWidth;
        Vector3 left = position - transform.right * gizmoWidth;
        Vector3 up = position + transform.up * gizmoWidth;
        Vector3 down = position - transform.up * gizmoWidth;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(position, front);
        Gizmos.DrawLine(left, front);
        Gizmos.DrawLine(right, front);
        Gizmos.DrawLine(up, front);
        Gizmos.DrawLine(down, front);

        Gizmos.DrawLine(up, right);
        Gizmos.DrawLine(right, down);
        Gizmos.DrawLine(down, left);
        Gizmos.DrawLine(left, up);
    }
}
