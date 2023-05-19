/*
    Authors: Jose Gonzalez, Ian Rodriguez
    Script Description:
        
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONWrapper<T>
{
    public T[] data;
}

public class JSONParser
{
    public static JSONWrapper<ObjectData> FromJSON(string jsonString)
    {
        return JsonUtility.FromJson<JSONWrapper<ObjectData>>(jsonString);
    }
}
