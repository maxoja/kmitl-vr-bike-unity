using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class PathController : MonoBehaviour 
{
    public Color roadColor = Color.grey;
    public Color bendPointColor = Color.cyan;
    public Color pathPointColor = Color.magenta;
    public Color pathLineColor = Color.yellow;

    [Range(0.3f, 10)]
    public float drawGap = 1f;
    [Range(0.1f, 20)]
    public float roadWidth = 4;

    public PathPoint[] points;

    private void ExtractPositions()
    {
        points = GetComponentsInChildren<PathPoint>();
    }

    private void OnDrawGizmos()
    {
        //return;
        ExtractPositions();

        if (points.Length < 2)
            return;
        
        PathPoint prevPoint = points[0];
        Vector3 lastDrawPoint = prevPoint.position;
        float drawDistance = 0;

        foreach (PathPoint point in points)
        {
            if (point == points[0]) continue;

            GameObject tempObject = new GameObject();
            Transform tempTrans = tempObject.transform;
            tempTrans.position = prevPoint.position;
            tempTrans.rotation = prevPoint.directionPoint.rotation;

            Vector3 a = prevPoint.position;
            Vector3 prevPos = prevPoint.position;
            Vector3 bendPos; // set later
            Vector3 targetPos = point.position;

            Quaternion r = prevPoint.directionPoint.rotation;
            Quaternion bendRot; // set later
            Quaternion prevRot = prevPoint.directionPoint.rotation;
            Quaternion targetRot = point.directionPoint.rotation;

            if (point.hasBendPoint)
            {
                //has bend point
                DrawHelper.DrawBendLine(point.position, point.bendPoint.position, bendPointColor);
                bendPos = 2 * (point.bendPoint.position - point.position) + point.position;
                bendRot = point.bendPoint.rotation;
            }
            else
            {
                //no bend point
                bendPos = prevPoint.position;
                bendRot = prevPoint.directionPoint.rotation;
            }
            
            for (int i = 1; i <= 20; i++)
            {
                Vector3 latestPos = tempTrans.position;

                a = Vector3.Lerp(prevPos, bendPos, 1f / 20 * i);
                tempTrans.position = Vector3.Lerp(a, targetPos, 1f / 20 * i);

                r = Quaternion.Lerp(prevRot, bendRot, 1f / 20 * i);
                tempTrans.rotation = Quaternion.Lerp(r, targetRot, 1f / 20 * i);

                float distance = Vector3.Distance(tempTrans.position, latestPos);
                point.AddDistance(distance);
                drawDistance += distance;

                while (drawDistance >= drawGap)
                {
                    Vector3 adjusted = lastDrawPoint + (tempTrans.position - lastDrawPoint).normalized * drawGap;

                    DrawHelper.DrawPathLine(lastDrawPoint, adjusted, pathLineColor);
                    DrawHelper.DrawRoadBlock(adjusted, tempTrans, roadColor, roadWidth);

                    lastDrawPoint = adjusted;
                    drawDistance -= drawGap;
                }
            }

            prevPoint = point;
            DestroyImmediate(tempObject);

        }
    }

    private static class DrawHelper
    {
        public static void DrawRoadBlock(Vector3 position, Transform trans, Color color, float width)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(position, position + trans.right * width);
            Gizmos.DrawLine(position, position - trans.right * width);
        }

        public static void DrawPathLine(Vector3 a, Vector3 b, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(a, b);
        }

        public static void DrawBendLine(Vector3 a, Vector3 b, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(a, b);
        }
    }
}


