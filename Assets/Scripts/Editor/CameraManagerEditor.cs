using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var tgt = target as CameraManager;
        if(tgt == null) {
            return;
        }

        tgt.FocalLength = EditorGUILayout.FloatField("Focal Length", tgt.FocalLength);
        tgt.SensorSize = EditorGUILayout.Vector2Field("Sensor Size", tgt.SensorSize);
        EditorUtility.SetDirty(target);
    }

}