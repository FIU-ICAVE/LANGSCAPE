/*
    Authors: Jose Gonzalez, Ian Rodriguez
    Script Description:
        
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ObjectType
{
    Block,
    Window
}
public enum ParseSuccess
{
    False = 0,
    True = 1
}

[System.Serializable]
public class ObjectData
{
    public bool success = false;
    public string type = "block";
    public Color color = Color.black;
    public Vector3 position;
}
