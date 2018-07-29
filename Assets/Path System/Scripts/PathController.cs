using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class PathController : MonoBehaviour 
{
    private Color roadColor = Color.grey;
    private Color bendPointColor = Color.cyan;
    private Color pathPointColor = Color.magenta;
    private Color pathLineColor = Color.yellow;

    [Range(0.3f, 10)]
    public float drawGap = 1f;
    [Range(0.1f, 20)]
    public float roadWidth = 4;
    [Range(10, 40)]
    public int pathQuality = 20;

    private PathPoint[] points = null;
    public float currentValue = 0f;
    public Transform movingTrans;

    private void Start()
    {
        StartCoroutine(UpdateCoroutine());
    }

    private IEnumerator UpdateCoroutine()
    {
        yield return new WaitUntil(() => points != null);
        //move
        while (true)
        {
            currentValue += Time.deltaTime * 40;
            float total = GetTotalDistance();
            while (currentValue >= total)
                currentValue -= total;

            float ratiop = 0;
            PathPoint pointp = null;
            GetMixture(currentValue, ref pointp, ref ratiop);

            Vector3 bp = 2 * (pointp.bendPoint.position - pointp.position) + pointp.position;
            Vector3 ap = Vector3.Lerp(pointp.sourcePosition, bp, ratiop);
            Quaternion rp = Quaternion.Lerp(pointp.sourceRotation, pointp.bendPoint.rotation, ratiop);


            movingTrans.position = Vector3.Lerp(ap, pointp.position, ratiop);
            movingTrans.rotation = Quaternion.Lerp(rp, pointp.rotation, ratiop);
            yield return null;
        }
    }
    private float GetTotalDistance(){
        float total = 0;
        foreach (PathPoint point in points)
            total += point.GetDistance();
        return total;
    }

    private void GetMixture(float leftover, ref PathPoint returnPoint, ref float returnRatio)
    {
        foreach(PathPoint point in points)
        {
            if(leftover < point.GetDistance())
            {
                returnPoint = point;
                returnRatio = leftover / point.GetDistance();
                return;
            }

            leftover -= point.GetDistance();
        }
    }

    private void OnDrawGizmos()
    {
        ExtractPoints();

        if (TooFewPoints())
            return;
        
        Vector3 launchPos = points[0].position;
        Vector3 latestMarkPos = launchPos;

        float distanceSinceLatestMark = 0;

        foreach (PathPoint currentTarget in points)
        {
            if (IsFirstPoint(currentTarget)) 
                continue;

            float cumulativeDistance = 0;

            Transform tempTrans = new GameObject().transform;
            tempTrans.parent = null;
            tempTrans.position = currentTarget.sourcePosition;
            tempTrans.rotation = currentTarget.sourceRotation;

            Vector3 a = currentTarget.sourcePosition;
            Vector3 prevPos = currentTarget.sourcePosition;
            Vector3 targetPos = currentTarget.position;

            Quaternion r = currentTarget.sourceRotation;
            Quaternion prevRot = currentTarget.sourceRotation;
            Quaternion targetRot = currentTarget.directionPoint.rotation;

            Vector3 bendPos; // set later
            Quaternion bendRot; // set later

            if (currentTarget.hasBendPoint)
            {
                //has bend point
                DrawBendLine(currentTarget.position, currentTarget.bendPoint.position);
                bendPos = 2 * (currentTarget.bendPoint.position - currentTarget.position) + currentTarget.position;
                bendRot = currentTarget.bendPoint.rotation;
                bendRot = Quaternion.LerpUnclamped(currentTarget.rotation, currentTarget.bendPoint.rotation, 2);
            }
            else
            {
                //no bend point
                bendPos = currentTarget.sourcePosition;
                bendRot = currentTarget.sourceRotation;
            }
            
            for (int i = 1; i <= pathQuality; i++)
            {
                float lerpRatio = 1f/pathQuality * i;
                Vector3 latestPos = tempTrans.position;

                a = Vector3.Lerp(prevPos, bendPos, lerpRatio);
                tempTrans.position = Vector3.Lerp(a, targetPos, lerpRatio);

                r = Quaternion.Lerp(prevRot, bendRot, lerpRatio);
                tempTrans.rotation = Quaternion.Lerp(r, targetRot, lerpRatio);

                float distance = Vector3.Distance(tempTrans.position, latestPos);
                cumulativeDistance += distance;
                distanceSinceLatestMark += distance;

                while (distanceSinceLatestMark >= drawGap)
                {
                    Vector3 adjusted = latestMarkPos + (tempTrans.position - latestMarkPos).normalized * drawGap;

                    DrawPathLine(latestMarkPos, adjusted);
                    DrawRoadBlock(adjusted, tempTrans);

                    latestMarkPos = adjusted;
                    distanceSinceLatestMark -= drawGap;
                }
            }

            currentTarget.SetDistance(cumulativeDistance);
            DestroyImmediate(tempTrans.gameObject);
        }
    }

    // HELPERs
    private bool TooFewPoints()
    {
        return points.Length < 2;
    }

    private bool IsFirstPoint(PathPoint point)
    {
        if (points == null || points.Length == 0)
            return false;
        else
            return point == points[0];
    }

    // EXTRACTION
    private void ExtractPoints()
    {
        points = GetComponentsInChildren<PathPoint>();
    }

    // GIZMOS DRAW HELPERs
    private void DrawRoadBlock(Vector3 position, Transform trans)
    {
        return;
        Gizmos.color = roadColor;
        Gizmos.DrawLine(position, position + trans.right * roadWidth);
        Gizmos.DrawLine(position, position - trans.right * roadWidth);

        //return;
        Gizmos.color = pathLineColor;
        Gizmos.DrawLine(position, position - trans.up * roadWidth / 3);

    }

    private void DrawPathLine(Vector3 a, Vector3 b)
    {
        return;
        Gizmos.color = pathLineColor;
        Gizmos.DrawLine(a, b);
    }

    private void DrawBendLine(Vector3 a, Vector3 b)
    {
        return;
        Gizmos.color = bendPointColor;
        Gizmos.DrawLine(a, b);
    }
}


