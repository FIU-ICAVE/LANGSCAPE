/*
    Author: Ian Rodriguez
    Script Description:
        This manager class provides several utilties for the project.
        Currently, it:
            - Executes all the commands & keeps a stack of commands to support undo/redo functionality.
            - Keeps track of various world statistics (such as total number of objects in the world).
    NOTES:
        - consider making a GameManager to Init properties in this class
    TO DO:
        - (maybe) implement some sort of threshold for checking if new pos is already taken
        - allow AI to query WorldState properties
        - research how to save world state along with properties in this class
        - GetWorldObjOfMaterial()
*/
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    /*
        World statistics fields
    */
    public int TotalNumWorldObjects {  get; private set; }

    /*
        Singleton
    */
    private static WorldStateManager _instance;
    public static WorldStateManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("WorldStateManager::Instance - WorldStateManager is null");
            }

            return _instance;
        }
    }


    private void Awake()
    {
        //singleton
        _instance = this;

        //initializing world stats
        TotalNumWorldObjects = 0;
    }

    /*
        Command stack
            For undoing/redoing commands.
    */
    private Stack<string> commandStack = new Stack<string>();
    private Stack<string> undoCommandStack = new Stack<string>();
    public void ExecutePush(Command cmd)
    {
        cmd.Execute();
        commandStack.Push(cmd.ToString());
    }
    public void RedoPush(Command cmd)
    {
        cmd.Redo();
        commandStack.Push(cmd.ToString());

        undoCommandStack.Pop();
    }
    public void UndoPop(Command cmd)
    {
        cmd.Undo();
        commandStack.Pop();

        undoCommandStack.Push(cmd.ToString());
    }
    public void BuildCommand(string message) //located here to avoid repeating this code throughout other scripts
    {
        int parseCode = CommandParser.Parse(message, '\n', ' ', out Command[] cmds);
        if (parseCode == 0)
        {
            foreach (Command cmd in cmds)
            {
                ExecutePush(cmd);
            }
            GridMesh.Instance.RegenerateMesh();
        }
    }

    /*
        World statistics methods
    */
    //[[[get data from "private GridCellData[,,] data;" when GridMesh.RegenerateMesh() is called, since it already loops through GridCellData[,,] ]]]
    public void UpdateTotalWorldObjects()
    {
        TotalNumWorldObjects++;
    }
    public int GetWorldObjOfMaterial(Material mat) //[[[TO DO]]]
    {
        //[[[how does this work with the single mesh implementation?]]]
        return 0;
    }

    /*
        Utility methods
    */
    public string[] GetCommandStackAsArray()
    {
        return commandStack.ToArray();
    }
    public string[] GetUndoStackAsArray()
    {
        return undoCommandStack.ToArray();
    }
}
