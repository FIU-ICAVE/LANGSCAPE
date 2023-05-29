/*
    Authors: Jose Gonzalez, Ian Rodriguez
    Script Description:
        [[[TO DO]]]
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
        JSONWrapper<ObjectData> data = new JSONWrapper<ObjectData>();
        try
        {
            data = JsonUtility.FromJson<JSONWrapper<ObjectData>>(jsonString);
        } 
        catch(System.Exception exception)
        {
            Debug.LogError(exception.Message);
        }

        return data;
    }

    public static string ToJSON(ObjectData obj) => JsonUtility.ToJson(obj);
}
