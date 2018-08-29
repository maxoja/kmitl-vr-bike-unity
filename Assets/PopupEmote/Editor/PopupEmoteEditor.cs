using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PopupEmote))]
[CanEditMultipleObjects]
public class PopupEmoteEditor : Editor {
    private SerializedProperty _emotePrefab;
    private SerializedProperty _popupAnimation;
    private SerializedProperty _builtInSprites;
    private SerializedProperty _customSprites;
    private SerializedProperty _emoteOrigin;
    private SerializedProperty _offset;
    private SerializedProperty _scale;

    void OnEnable() {
        _emotePrefab = serializedObject.FindProperty("_emotePrefab");
        _popupAnimation = serializedObject.FindProperty("_popupAnimation");
        _builtInSprites = serializedObject.FindProperty("_builtInSprites");
        _customSprites = serializedObject.FindProperty("_customSprites");
        _emoteOrigin = serializedObject.FindProperty("_emoteOrigin");
        _offset = serializedObject.FindProperty("_offset");
        _scale = serializedObject.FindProperty("_scale");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        PopupEmote emote = (PopupEmote) target;

        EditorGUILayout.PropertyField(_emotePrefab);
        EditorGUILayout.PropertyField(_popupAnimation);

        int choiceIndex = EditorGUILayout.Popup(new GUIContent("Emote Set"), emote.SelectedTextureIndex, emote.assetPaths.Select(n => new GUIContent(Path.GetFileNameWithoutExtension(n))).ToArray());
        emote.SelectTexture(choiceIndex);

        // Render the built-in sprites as a normal array, but disable the GUI element so that
        // it's not editable
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.indentLevel++;
        GUI.enabled = false;
        EditorGUILayout.PropertyField(_builtInSprites, new GUIContent("Built-In Emotes"), true);
        GUI.enabled = true;
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_customSprites, new GUIContent("Custom Emotes"), true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(_emoteOrigin);
        EditorGUILayout.PropertyField(_offset);
        EditorGUILayout.PropertyField(_scale);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Reload Content")) {
            emote.Reload();
        }
    }
}
