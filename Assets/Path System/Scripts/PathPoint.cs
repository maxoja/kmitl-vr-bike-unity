using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathPoint : MonoBehaviour 
{
    [HideInInspector]
    public PointType type = PointType.WayPoint;
    public PathPoint sourcePoint = null;
    public bool hasBendPoint = false;
    public bool isLaunchPoint = false;
    public float bendingFactor = 2;
    public float lerpFactor = 1.5f;


    public Vector3 controlPointPos = new Vector3();

    private float distance = 0;
    private const float gizmoLength = 4;
    private const float gizmoWidth = 0.5f;

    public Vector3 position { get { return transform.position; } }
    //public Quaternion rotation { get { return this.targetRotation; } }
    public Vector3 up { get { return transform.up; } }
    public Vector3 right { get { return transform.right; } }
    public Vector3 forward { get { return transform.forward; } }
    protected Vector3 controlPositionFront { get { return controlPointPos + position; } }
    protected Vector3 controlPositionBack { get { return position - controlPointPos; } }
    public Vector3 controlPositionA { get { return sourcePoint.controlPositionFront; }}
    public Vector3 controlPositionB { get { return this.controlPositionBack; }}

    public Vector3 sourcePosition { 
        get { 
            if (sourcePoint == null) 
                return Vector3.zero;
            return sourcePoint.position; 
        }
    }
    //public Quaternion sourceRotation { 
    //   get { 
    //        if(sourcePoint == null)
    //            return Quaternion.identity;
    //        return sourcePoint.rotation; 
    //    }
    //}
    public Vector3 targetPosition{get{return transform.position;}}
    //public Quaternion targetRotation{get{return transform.rotation;}}
    //public Vector3 bendPosition
    //{
    //    get {
    //        if (sourcePoint == null)
    //            return Vector3.zero;
    //        return ExtractBendingPosition();
    //    }
    //}

    //public Quaternion bendRotation{get{return sourceRotation;}}

    private void OnDrawGizmos()
    {
        //return;
        //draw sphere
        Color temp = Color.magenta * 2;
        temp.a = 0.2f;
        Gizmos.color = temp;

        Gizmos.DrawSphere(position, 0.7f);

        //prep to draw arrow
        Vector3 front = position + transform.forward * gizmoLength;
        Vector3 right = position + transform.right * gizmoWidth;
        Vector3 left = position - transform.right * gizmoWidth;
        Vector3 up = position + transform.up * gizmoWidth;
        Vector3 down = position - transform.up * gizmoWidth;

        //draw arrow
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
        WayPoint,
    }

    private Vector3 ExtractBendingPosition()
    {
        Vector3 sourceHitPoint, targetHitPoint;

        Vector3 sourcePosA = sourcePosition;
        Vector3 sourcePosB = sourcePosA + sourcePoint.forward * 10000;

        Vector3 targetPosA = position;
        Vector3 targetPosB = targetPosA - forward * 10000;

        bool parallel = !ClosestPointsOnTwoLines(out sourceHitPoint, out targetHitPoint, sourcePosA, sourcePosB, targetPosA, targetPosB);

        if (!parallel)
        {
            Vector3 mid = Vector3.Lerp(sourceHitPoint, targetHitPoint, 0.5f);
            Vector3 difference = (mid - position);
            Vector3 bendPos = position + difference * bendingFactor;

            return bendPos;
        }

        return sourcePosition;
    }

    //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
    //to each other. This function finds those two points. If the lines are not parallel, the function 
    //outputs true, otherwise false.
    private bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
 
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;
 
        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);
 
        float d = a*e - b*b;
 
        //lines are not parallel
        if(d != 0.0f){
 
            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);
 
            float s = (b*f - c*e) / d;
            float t = (a*f - c*b) / d;
 
            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;
 
            return true;
        }
 
        else{
            return false;
        }
    }   
}
