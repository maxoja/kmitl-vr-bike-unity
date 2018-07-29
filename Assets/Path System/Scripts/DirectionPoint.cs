using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DirectionPoint : MonoBehaviour 
{
    public Quaternion rotation { get { return transform.rotation; } }
    public Vector3 forward { get { return transform.forward; } }

    private const float gizmoLength = 4;
    private const float gizmoWidth = 0.5f;

    private void Update()
    {
        transform.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Vector3 front = pos + transform.forward * gizmoLength;
        Vector3 right = pos + transform.right * gizmoWidth;
        Vector3 left = pos - transform.right * gizmoWidth;
        Vector3 up = pos + transform.up * gizmoWidth;
        Vector3 down = pos - transform.up * gizmoWidth;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(pos, front);
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
