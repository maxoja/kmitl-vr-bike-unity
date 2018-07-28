using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathPoint : MonoBehaviour 
{
    [HideInInspector]
    public PointType type = PointType.WithBendPoint;
    public PathPoint beginPoint;
    public BendPoint bendPoint;
    public DirectionPoint directionPoint = null;
    public bool hasBendPoint = false;
    public bool startPoint = false;

    private float distance = 0;

    public Vector3 position { get { return transform.position; } }
    //public Quaternion rotation { get { return transform.rotation; } }
    public Vector3 up { get { return transform.up; } }
    public Vector3 right { get { return transform.right; } }
    public Vector3 forward { get { return transform.forward; } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(position, 1);
    }

    public float GetDistance()
    {
        return distance;
    }

    public void SetDistance(float newDistance)
    {
        this.distance = newDistance;
    }

    public void AddDistance(float moreDistance)
    {
        this.distance += moreDistance;
    }

    public enum PointType
    {
        StartPoint,
        WithBendPoint,
        WithoutBendPoint
    }
}
