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
                script.startPoint = true;
                script.hasBendPoint = false;

                RemoveBendPoint(script);
                break;

            case PathPoint.PointType.WithBendPoint:
                script.hasBendPoint = true;
                script.startPoint = false;

                ExtractBendPoint(script);

                DrawBeginPointField(script);
                DrawBendPointField(script);
                break;

            case PathPoint.PointType.WithoutBendPoint:
                script.hasBendPoint = false;
                script.startPoint = false;

                RemoveBendPoint(script);

                DrawBeginPointField(script);
                break;
        }

    }

    // EXTRACTION //
    private void ExtractBeginningPoint(PathPoint script)
    {
        int siblingId = script.transform.GetSiblingIndex();
        if (siblingId > 0)
            script.beginPoint = script.transform.parent.GetChild(siblingId - 1).GetComponent<PathPoint>();
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
            //newObject.transform.position = Vector3.Lerp(script.bendPoint.position, script.position, 0.5f);
            script.bendPoint = newObject.AddComponent<BendPoint>();
        }
    }
    private void RemoveBendPoint(PathPoint script)
    {
        script.bendPoint = script.GetComponentInChildren<BendPoint>();

        if (script.bendPoint != null)
        {
            Debug.Log("delete bend point");
            DestroyImmediate(script.bendPoint.gameObject);
        }

        script.bendPoint = null;
    }

    // GUI DRAWING //
    private void DrawTypeField(PathPoint script)
    {
        script.type = (PathPoint.PointType)EditorGUILayout.EnumPopup("Point Type", script.type);
    }

    private void DrawBeginPointField(PathPoint script)
    {
        EditorGUILayout.ObjectField("Begin Point",script.beginPoint, typeof(PathPoint), true);
    }

    private void DrawBendPointField(PathPoint script)
    {
        EditorGUILayout.ObjectField("Bend Point", script.bendPoint, typeof(BendPoint), true);
    }
}
