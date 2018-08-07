using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

//todo steady speed
//done fix imperfect joint
//comment to create checkpoint

//[ExecuteInEditMode]
public class PathController : MonoBehaviour 
{
    private static Color roadColor = Color.grey;
    private static Color bendPointColor = Color.cyan*2;
    private static Color pathPointColor = Color.magenta*2;
    private static Color pathLineColor = Color.yellow*2;

    //[SerializeField]
    //private float factor = 1.5f;
    //[SerializeField]
    //[Range(-2,0)]
    //public float factor2 = 10;

    [Range(0.3f, 10)]
    public float drawBlockGap = 1f;
    [Range(0.1f, 20)]
    public float roadWidth = 4;
    [Range(5, 40)]
    public int pathQuality = 20;
    [Range(1, 50)]
    public int travelSpeed = 40;
    [Range(1, 10)]
    public int turnSoftness = 5;

    private PathPoint[] points = null;
    private float currentValue = 0f;
    public Transform movingTrans;

    [SerializeField]
    private bool showPathLine = true;
    [SerializeField]
    private bool showPerpendicularLine = true;
    [SerializeField]
    private bool showRoad = true;
    [SerializeField]
    private bool showBendLine = true;
    [SerializeField]
    private bool showDots = true;

    private void Start()
    {
        StartCoroutine(UpdateCoroutine());
    }

    private IEnumerator UpdateCoroutine()
    {
        yield return new WaitUntil(() => points != null);
        float total = GetTotalDistance();
        //move
        while (true)
        {
            currentValue += Time.deltaTime * travelSpeed;

            while (currentValue >= total)
                currentValue -= total;

            float ratio = 0;
            PathPoint point = null;
            GetMixture(currentValue, ref point, ref ratio);

            movingTrans.position = GetPosOf(point, ratio);
            //movingTrans.rotation = Quaternion.Lerp(movingTrans.rotation, GetRotOf(point, ratio), Time.deltaTime*travelSpeed/turnSoftness);

            GetMixture(currentValue+0.5f, ref point, ref ratio);
            movingTrans.LookAt(GetPosOf(point, ratio));

            yield return null;
        }
    }
    private Vector3 QuadraticLerp(Vector3 a, Vector3 control, Vector3 b, float percentage)
    {
        Vector3 p1 = Vector3.Lerp(a, control, percentage);
        Vector3 p2 = Vector3.Lerp(control, b, percentage);
        return Vector3.Lerp(p1, p2, percentage);

        Vector3 p0 = Vector3.Lerp(a, control, percentage);
        return Vector3.Lerp(p0, b, percentage);
    }

    //private Quaternion GetRotOf(PathPoint point, float ratio)
    //{
    //    Quaternion pinRot = Quaternion.Lerp(point.sourceRotation, point.bendRotation, ratio);
    //    Quaternion resultRot = Quaternion.Lerp(pinRot, point.targetRotation, ratio);
    //    return resultRot;
    //}

    private Vector3 GetPosOf(PathPoint point, float percentage)
    {
        Vector3 a = point.sourcePosition;
        Vector3 b = point.controlPositionA;
        Vector3 c = point.controlPositionB;
        Vector3 d = point.position;

        Vector3 p1 = QuadraticLerp(a, b, c, percentage);
        Vector3 p2 = QuadraticLerp(b, c, d, percentage);

        Vector3 resultPos = Vector3.Lerp(p1, p2, percentage);
        return resultPos;
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
                returnRatio = GetAdjustedLerpRatio(leftover / point.GetDistance(), point);
                return;
            }

            leftover -= point.GetDistance();
        }
    }

    private float GetAdjustedLerpRatio(float ratio, PathPoint point)
    {
        return ratio;
        float distA = Vector3.Distance(point.sourcePosition, point.controlPositionA);
        float distB = Vector3.Distance(point.targetPosition, point.controlPositionB);
        float deltaDist = distA - distB;
        return Mathf.Pow(ratio, 1 + deltaDist*0.025f);
        //return Mathf.Pow(ratio, point.lerpFactor);
    }

    private void OnDrawGizmos()
    {
        ExtractPoints();

        if (TooFewPoints())
            return;
        
        Vector3 launchPos = points[0].position;
        Vector3 latestMarkPos = launchPos;

        float distanceSinceLatestMark = 0;

        foreach (PathPoint targetPoint in points)
        {
            if (IsFirstPoint(targetPoint)) 
                continue;

            float cumulativeDistance = 0;
            Transform tempTrans = CreateTempTransform(targetPoint);

            // note : now, every path point which is not start point has bend point
            // prepare vars using for pos calculation //
            Vector3 sourcePos = targetPoint.sourcePosition;
            Vector3 controlPosA = targetPoint.controlPositionA;
            Vector3 controlPosB = targetPoint.controlPositionB;
            Vector3 targetPos = targetPoint.position;
            Vector3 resultPos = Vector3.zero;

            //DrawBendLine(targetPoint.position, targetPoint.bendPosition);
            //DrawBendLine(targetPoint.position, bendPos);

            // prepare vars using for rot calculation //
            //Quaternion sourceRot = targetPoint.sourceRotation;
            //Quaternion pinRot = targetPoint.sourceRotation;
            //Quaternion targetRot = targetPoint.targetRotation;
            //Quaternion resultRot = Quaternion.identity;
            //Quaternion bendRot = targetPoint.bendRotation;

            string result = "";
            for (int i = 1; i <= pathQuality; i++)
            {
                float baseRatio = i * (1f / pathQuality);
                float lerpRatio = GetAdjustedLerpRatio(baseRatio, targetPoint);

                Vector3 latestPrevPos = tempTrans.position;

                //calculate
                //pinRot = Quaternion.Lerp(sourceRot, bendRot, lerpRatio);
                resultPos = GetPosOf(targetPoint, lerpRatio);
                //resultRot = Quaternion.Lerp(pinRot, targetRot, lerpRatio);

                //set calculated values to transform
                tempTrans.position = resultPos;
                //tempTrans.rotation = resultRot;
                float occurredDistance = Vector3.Distance(latestPrevPos, resultPos);
                cumulativeDistance += occurredDistance;
                distanceSinceLatestMark += occurredDistance;

                //draw gizmos
                DrawPathLine(latestPrevPos, resultPos);
                DrawBlocks(ref distanceSinceLatestMark, ref latestMarkPos, tempTrans);

                result += cumulativeDistance;
                result += ", ";
            }
            result += targetPoint.gameObject.name;
            Debug.Log(result);

            targetPoint.SetDistance(cumulativeDistance);
            DestroyImmediate(tempTrans.gameObject);
        }
    }

    // HELPERs
    private Transform CreateTempTransform(PathPoint target)
    {
        Transform t = new GameObject().transform;
        t.parent = null;
        t.position = target.sourcePosition;
        //t.rotation = target.sourceRotation;

        return t;
    }

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

    private void ExtractPoints()
    {
        points = GetComponentsInChildren<PathPoint>();
    }


    // GIZMOS DRAW HELPERs
    private void DrawBlocks(ref float distanceSinceLatestMark, ref Vector3 latestMarkPos, Transform tempTrans)
    {
        if (distanceSinceLatestMark < drawBlockGap)
            return;

        Vector3 offset = (tempTrans.position - latestMarkPos).normalized * drawBlockGap;
        Vector3 drawPos = latestMarkPos + offset;

        DrawRoadBlock(drawPos, tempTrans);

        distanceSinceLatestMark -= drawBlockGap;
        latestMarkPos = drawPos;

        DrawBlocks(ref distanceSinceLatestMark, ref latestMarkPos, tempTrans);
    }

    private void DrawRoadBlock(Vector3 position, Transform trans)
    {
        if (showRoad)
        {
            Gizmos.color = roadColor;
            Gizmos.DrawLine(position, position + trans.right * roadWidth);
            Gizmos.DrawLine(position, position - trans.right * roadWidth);
        }

        if (showPerpendicularLine)
        {
            Gizmos.color = pathLineColor;
            Gizmos.DrawLine(position, position - trans.up * roadWidth / 3);
        }
    }

    private void DrawPathLine(Vector3 a, Vector3 b)
    {
        if (showPathLine)
        {
            Gizmos.color = pathLineColor;
            Gizmos.DrawLine(a, b);
        }

        if (showDots)
        {
            Gizmos.color = Color.white * 2;
            Gizmos.DrawSphere(a, 0.2f);
        }
    }

    private void DrawBendLine(Vector3 a, Vector3 b)
    {
        if (!showBendLine)
            return;
        Gizmos.color = bendPointColor*2;

        Vector3 knobPos = Vector3.Lerp(a, b, 0.4f);
        Gizmos.DrawLine(a, knobPos);
        Gizmos.DrawSphere(knobPos, 0.2f);
    }
}


