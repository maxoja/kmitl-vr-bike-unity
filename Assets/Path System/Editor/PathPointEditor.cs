using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathPoint))]
public class PathPointEditor : Editor {

    protected virtual void OnSceneGUI(){
        PathPoint script = target as PathPoint;

        //if (Tools.current != Tool.Move)
            //return;
        
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(script.controlPointPos, Quaternion.identity);
        Handles.PositionHandle(script.position - (script.controlPointPos-script.position), Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Change S");
            script.controlPointPos = newTargetPosition;
        }
    }
    public override void OnInspectorGUI()
    {
        PathPoint script = target as PathPoint;

        ExtractBeginningPoint(script);

        DrawTypeField(script);

        switch (script.type)
        {
            case PathPoint.PointType.StartPoint:
                script.isLaunchPoint = true;
                script.hasBendPoint = false;
                break;

            case PathPoint.PointType.WayPoint:
                script.hasBendPoint = true;
                script.isLaunchPoint = false;

                DrawBeginPointField(script);
                DrawBendingFactorField(script);
                DrawLerpFactorField(script);
                DrawDistanceField(script);
                break;
        }

        DrawS(script);

        //refresh PathController
        SceneView.RepaintAll();
    }

    // EXTRACTION //
    private void ExtractBeginningPoint(PathPoint script)
    {
        int siblingId = script.transform.GetSiblingIndex();
        if (siblingId > 0)
            script.sourcePoint = script.transform.parent.GetChild(siblingId - 1).GetComponent<PathPoint>();
    }

    // GUI DRAWING //
    private void DrawBendingFactorField(PathPoint script)
    {
        script.bendingFactor = EditorGUILayout.Slider("Bending Factor", script.bendingFactor, 1f, 3f);
    }
    private void DrawLerpFactorField(PathPoint script)
    {
        script.lerpFactor = EditorGUILayout.Slider("Lerp Factor", script.lerpFactor, 0.5f, 2f);
    }
    private void DrawDistanceField(PathPoint script)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField("Distance from source",script.GetDistance());
        EditorGUI.EndDisabledGroup();
    }
    private void DrawTypeField(PathPoint script)
    {
        script.type = (PathPoint.PointType)EditorGUILayout.EnumPopup("Point Type", script.type);
    }
    private void DrawBeginPointField(PathPoint script)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Source Point",script.sourcePoint, typeof(PathPoint), true);
        EditorGUI.EndDisabledGroup();
    }
    private void DrawS(PathPoint script)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Vector3Field("S", script.controlPointPos);
        EditorGUI.EndDisabledGroup();
    }
}

