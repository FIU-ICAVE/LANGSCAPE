//
// Authors: Jose Gonzalez
//
// Description: Creates a button at runtime inside the editor that allows
// us to create a block without having to call the AI. Excluded from build.
//

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMesh))]
public class GridTestingEditor : Editor {
    public override void OnInspectorGUI () {
        DrawDefaultInspector();

        if (GridMesh.Instance != null) {
            if (GUILayout.Button("Run Command"))
                GridMesh.TestCommandViaEditor();
        }

        EditorGUILayout.HelpBox("To see command syntax, please take a look at the respective command files located under [Assets > Scripts > AI > Commands] which should contain them in comments.", MessageType.Info);
    }
}
