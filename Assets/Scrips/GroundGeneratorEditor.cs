using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundGenerator))]
public class GroundGeneratorEditor : Editor {
    private SerializedProperty _groundSprites;
    private SerializedProperty _gridSize;
    private SerializedProperty _unitSize;

    private void OnEnable() {
        _groundSprites = serializedObject.FindProperty("groundSprites");
        _gridSize = serializedObject.FindProperty("gridSize");
        _unitSize = serializedObject.FindProperty("unitSize");
    }

    public override void OnInspectorGUI() {
        GroundGenerator worldGenerator = (GroundGenerator)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(_groundSprites);
        EditorGUILayout.PropertyField(_gridSize);
        EditorGUILayout.PropertyField(_unitSize);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate Ground")) {
            Debug.Log("Hello");
            worldGenerator.GenerateGround();
        }
    }
}
