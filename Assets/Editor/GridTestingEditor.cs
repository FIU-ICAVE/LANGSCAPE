using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMesh))]
public class GridTestingEditor : Editor {
    public override void OnInspectorGUI () {
        DrawDefaultInspector();

        if (GridMesh.Instance != null) {
            if (GUILayout.Button("Place Object"))
                GridMesh.TestPlacement();
        }
    }
}
