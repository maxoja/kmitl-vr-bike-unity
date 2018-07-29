using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathPoint))]
public class PathPointEditor : Editor {
    
    public override void OnInspectorGUI()
    {
        PathPoint script = target as PathPoint;

        ExtractBeginningPoint(script);
        ExtractDirectionPoint(script);

        DrawTypeField(script);

        switch (script.type)
        {
            case PathPoint.PointType.StartPoint:
                script.isLaunchPoint = true;
                script.hasBendPoint = false;

                RemoveBendPoint(script);
                break;

            case PathPoint.PointType.WithBendPoint:
                script.hasBendPoint = true;
                script.isLaunchPoint = false;

                ExtractBendPoint(script);

                DrawBeginPointField(script);
                DrawBendPointField(script);
                break;

            case PathPoint.PointType.WithoutBendPoint:
                script.hasBendPoint = false;
                script.isLaunchPoint = false;

                RemoveBendPoint(script);

                DrawBeginPointField(script);
                break;
        }

        DrawDistanceField(script);
    }

    // EXTRACTION //
    private void ExtractBeginningPoint(PathPoint script)
    {
        int siblingId = script.transform.GetSiblingIndex();
        if (siblingId > 0)
            script.sourcePoint = script.transform.parent.GetChild(siblingId - 1).GetComponent<PathPoint>();
    }
    private void ExtractDirectionPoint(PathPoint script)
    {
        script.directionPoint = script.GetComponentInChildren<DirectionPoint>();

        if ( script.directionPoint == null )
        {
            GameObject newObject = new GameObject("direction");
            newObject.transform.parent = script.transform;
            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localRotation = Quaternion.identity;
            script.directionPoint = newObject.AddComponent<DirectionPoint>();
        }
    }
    private void ExtractBendPoint(PathPoint script)
    {
        //return;   
        script.bendPoint = script.GetComponentInChildren<BendPoint>();

        if ( script.bendPoint == null )
        {
            GameObject newObject = new GameObject("bend");
            newObject.transform.parent = script.transform;
            newObject.transform.position = Vector3.Lerp(script.sourcePosition, script.position, 0.5f);
            newObject.transform.rotation = script.rotation;
            script.bendPoint = newObject.AddComponent<BendPoint>();
        }
    }
    private void RemoveBendPoint(PathPoint script)
    {
        script.bendPoint = script.GetComponentInChildren<BendPoint>();

        if (script.bendPoint != null)
            DestroyImmediate(script.bendPoint.gameObject);

        script.bendPoint = null;
    }

    // GUI DRAWING //
    private void DrawDistanceField(PathPoint script)
    {
        EditorGUILayout.FloatField("Distance from source",script.GetDistance());
    }
    private void DrawTypeField(PathPoint script)
    {
        script.type = (PathPoint.PointType)EditorGUILayout.EnumPopup("Point Type", script.type);
    }

    private void DrawBeginPointField(PathPoint script)
    {
        EditorGUILayout.ObjectField("Source Point",script.sourcePoint, typeof(PathPoint), true);
    }

    private void DrawBendPointField(PathPoint script)
    {
        EditorGUILayout.ObjectField("Bend Point", script.bendPoint, typeof(BendPoint), true);
    }
}
