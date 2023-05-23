using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

public class EditorUtils : EditorWindow {
    private void OnGUI() {
        // Use the Object Picker to select the start SceneAsset
        EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(
            new GUIContent("Start Scene"), EditorSceneManager.playModeStartScene, typeof(SceneAsset), false);
    }

    [MenuItem("EditorUtils/Open")]
    private static void Open() {
        GetWindow<EditorUtils>();
    }
}

