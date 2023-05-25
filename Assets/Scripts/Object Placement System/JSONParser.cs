/*
    Authors: Jose Gonzalez, Ian Rodriguez
    Script Description:
        
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class JSONWrapper<T>
{
    public T[] data;
}

[System.Serializable]
public class CommandObject {
    public BlockType type = 0;
    public Vector3Int original_position = new Vector3Int(-1, -1, -1);
    public Vector3Int new_position = new Vector3Int(-1, -1, -1);
    public Color color = Color.white;

    public CommandObject(BlockType type, Vector3Int original_position, Vector3Int new_position, Color color) {
        this.type = type;
        this.original_position = original_position;
        this.new_position = new_position;
        this.color = color;
    }
}

[System.Serializable]
public class WorldCommand {
    public bool success;
    public CommandObject[] modified;

    public WorldCommand(bool success, CommandObject[] modified) {
        this.success = success;
        this.modified = modified;
    }
}
public class JSONParser
{
    public static WorldCommand ParseCommand(string json) {
        WorldCommand commands = new WorldCommand(false, null);
        try {
            commands = JsonUtility.FromJson<WorldCommand>(json);
        } catch (System.Exception e) {
            Debug.Assert(false, e.Message);
        }
        return commands;
    }

    public static JSONWrapper<ObjectData> FromJSON(string jsonString)
    {
        return JsonUtility.FromJson<JSONWrapper<ObjectData>>(jsonString);
    }

    public static string ToJSON(ObjectData obj) => JsonUtility.ToJson(obj);
}
