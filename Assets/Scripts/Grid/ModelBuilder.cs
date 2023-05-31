using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelBuilder : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] indices;

/*    // Draws the outline of the world in the inspector
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (drawInEditor) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(-9.5f, -10.5f, -9.5f), Vector3.one);
            Handles.DrawAAConvexPolygon(new Vector3[] {
                Vector3.zero,
                new Vector3(initialWorldSize.x, 0, 0),
                new Vector3(initialWorldSize.x, 0, initialWorldSize.z),
                new Vector3(0, 0, initialWorldSize.z)
            });
        }
    }
#endif*/
}
