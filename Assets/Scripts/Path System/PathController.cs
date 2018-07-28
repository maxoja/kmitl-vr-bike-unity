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

    public Transform startTrans;
    public PathPointt[] points;

    private void ExtractPositions()
    {
        if (transform.childCount < 2)
            return;

        int numChild = transform.childCount;
        startTrans = transform.GetChild(0);
        points = new PathPointt[numChild-1];

        Transform prevTrans = startTrans;

        for (int i = 1; i < numChild; i++)
        {
            Transform child = transform.GetChild(i);

            PathPointt newPathPoint = new PathPointt();
            newPathPoint.targetTrans = child;

            if (child.childCount == 0)
            {
                newPathPoint.bendTrans = prevTrans;
                newPathPoint.hasBendPoint = false;
            }
            else
            {
                newPathPoint.bendTrans = child.GetChild(0);
                newPathPoint.hasBendPoint = true;
            }

            points[i-1] = newPathPoint;
            prevTrans = newPathPoint.targetTrans;
        }
    }

    private void OnDrawGizmos()
    {
        //return;
        ExtractPositions();

        Transform prevTrans = startTrans;
        float drawDistance = 0;
        Vector3 lastDrawPoint = prevTrans.position;


        foreach (PathPointt point in points)
        {
            DrawHelper.DrawBendLine(point.targetTrans.position, point.bendTrans.position, bendPointColor);

            GameObject tempObject = new GameObject();
            Transform tempTrans = tempObject.transform;
            tempTrans.position = prevTrans.position;
            tempTrans.rotation = prevTrans.rotation;

            Vector3 a = prevTrans.position;
            Vector3 prevPos = prevTrans.position;
            Vector3 bendPos = 2*(point.bendTrans.position - point.targetTrans.position) + point.targetTrans.position;
            Vector3 targetPos = point.targetTrans.position;

            Quaternion r = prevTrans.rotation;
            Quaternion bendRot = point.bendTrans.rotation;
            Quaternion prevRot = prevTrans.rotation;
            Quaternion targetRot = point.targetTrans.rotation;

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

            prevTrans = point.targetTrans;
            DestroyImmediate(tempObject);

        }
    }

    [System.Serializable]
    public class PathPointt
    {
        public Transform targetTrans;
        public Transform bendTrans;
        public bool hasBendPoint = false;

        private float distance = 0;

        public float GetDistance(){
            return distance;
        }

        public void SetDistance(float newDistance) {
            this.distance = newDistance;
        }

        public void AddDistance(float moreDistance) {
            this.distance += moreDistance;
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


