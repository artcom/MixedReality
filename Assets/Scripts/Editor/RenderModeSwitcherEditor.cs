using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RenderModeSwitcher))]
public class RenderModeSwitcherEditor : Editor {
    
    public override void OnInspectorGUI() {
        var tgt = target as RenderModeSwitcher;

        if(tgt == null) {
            GUILayout.Label("Editor is curtrently not available.");
        }

        DrawDefaultInspector();
        var val = (RenderModeSwitcher.RenderMode) EditorGUILayout.EnumPopup("Render Mode", tgt.CurrentRenderMode);
        if(val != tgt.CurrentRenderMode) {
            tgt.CurrentRenderMode = val;
        }
    }
}