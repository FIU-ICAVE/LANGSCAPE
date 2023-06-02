//
// Authors: Jose Gonzalez
//
// Description: Loads a text file in 'Assets/Resources' and reads it as JSON to
// obtain the vertex and face data of a model. These include the basic block and
// any other object the AI should be able to place.
//

using System;
using UnityEngine;

// TODO: add init method and load all the different models here, not in the mesh.
public class GridModel
{
    public Vector3[] vertices;
    public int[] front;
    public int[] back;
    public int[] left;
    public int[] right;
    public int[] top;
    public int[] bottom;

    public static GridModel Load(string filename) {
        if (filename == null) return null;

        TextAsset content = Resources.Load<TextAsset>(filename);
        if (content == null) return null;

        GridModel model;

        try {
            model = JsonUtility.FromJson<GridModel>(content.text);
        } catch (Exception e) {
            Debug.LogError(e.Message);
            model = null;
        }

        return model;
    }
}
