using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceData {
    public int[] indices;
}

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
