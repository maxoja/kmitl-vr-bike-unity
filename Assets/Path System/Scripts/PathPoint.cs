using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathPoint : MonoBehaviour 
{
    [HideInInspector]
    public PointType type = PointType.WithBendPoint;
    public PathPoint sourcePoint = null;
    public BendPoint bendPoint = null;
    public DirectionPoint directionPoint = null;
    public bool hasBendPoint = false;
    public bool isLaunchPoint = false;

    private float distance = 0;

    public Vector3 position { get { return transform.position; } }
    public Quaternion rotation { get { return directionPoint.rotation; } }
    //public Quaternion direction { get { return directionPoint.rotation; } }
    public Vector3 up { get { return transform.up; } }
    public Vector3 right { get { return transform.right; } }
    public Vector3 forward { get { return transform.forward; } }

    public Vector3 sourcePosition { 
        get { 
            if (sourcePoint == null) 
                return Vector3.zero;
            return sourcePoint.position; 
        }
    }
    public Quaternion sourceRotation { 
        get { 
            if(sourcePoint == null)
                return Quaternion.identity;
            return sourcePoint.rotation; 
        }
    }

    private void OnValidate()
    {
        transform.rotation = Quaternion.identity;    
    }

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

    public enum PointType
    {
        StartPoint,
        WithBendPoint,
        WithoutBendPoint
    }
}
