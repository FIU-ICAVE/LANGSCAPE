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
    public int TotalNumWorldObjects { get; private set; }

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
    private Stack<CommandBatch> commandStack = new Stack<CommandBatch>();
    private Stack<CommandBatch> undoStack = new Stack<CommandBatch>();

    public void ExecutePush(CommandBatch batch)
    {
        if (undoStack.Count == 0) 
            undoStack.Clear();
        batch.Execute();
        commandStack.Push(batch);
    }
    public void RedoCommandBatch(int count)
    {
        for (int i = 0; i < count; i++) {
            if (undoStack.Count == 0)
                return;
            CommandBatch batch = undoStack.Pop();
            batch.Redo();
            commandStack.Push(batch);
        }
    }
    public void UndoCommandBatch(int count)
    {
        for (int i = 0; i < count; i++) {
            if (commandStack.Count == 0)
                return;
            CommandBatch batch = commandStack.Pop();
            batch.Undo();
            undoStack.Push(batch);
        }
    }
    public void BuildCommandBatch(string response, string request) //located here to avoid repeating this code throughout other scripts
    {
        char[] split = {'\n', '\r'};
        int parseCode = CommandParser.Parse(response, split, ' ', out Command[] cmds);
        if (parseCode == 0 && cmds != null && cmds.Length != 0)
        {
            CommandBatch batch = new(cmds, request, response);
            ExecutePush(batch);
            GridMesh.Instance.RegenerateMesh();
        }
        if (parseCode != LangscapeError.CMD_VALID.code)
        {
            AIMic.Instance.ShushFluff();
        }
        LangscapeError.Instance.ThrowUserError(parseCode);
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
        Stack<string> commandStringStack = new Stack<string>();
        foreach (CommandBatch batch in commandStack)
        {
            foreach (Command cmd in batch.commands)
            {
                commandStringStack.Push(cmd.ToString());
            }
        }

        return commandStringStack.ToArray();
    }
    public string[] GetUndoStackAsArray()
    {
        Stack<string> undoStringStack = new Stack<string>();
        foreach (CommandBatch batch in commandStack)
        {
            foreach (Command cmd in batch.commands) 
            {
                undoStringStack.Push(cmd.ToString());
            }
        }

        return undoStringStack.ToArray();
    }
}
