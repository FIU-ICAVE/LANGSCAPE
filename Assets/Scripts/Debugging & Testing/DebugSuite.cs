/*
    Author: Ian Rodriguez
    Script Description:
        This class serves as a central hub for debugging all features of the project. All fields are *SET HERE*, for cohesiveness.
    NOTES:
        - consider moving methods in Update() to wherever they would be updated throughout the codebase (for performance)
            - i.e., consider moving FillCommandStacks() to whenever WorldStateManager's commandStack actually gets updated
*/
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugSuite : MonoBehaviour
{
#if UNITY_EDITOR

    private void Update()
    {
        //command stacks
        FillCommandStacks();

        //world statistics
        totalWorldObjects = WorldStateManager.Instance.TotalNumWorldObjects;
    }

    [Header("[[[REMIND IAN LATER THIS IS NOT FINISHED!]]]")]

    /*
    [SerializeField] private bool debugAll = false;
    private void EnableAllFeatures()
    {
        if(debugAll)
        {
            commandStackDebug = true;
            worldStatisticsDebug = true;
        }
    }*/

    [Header("Command Stacks (as arrays)")]
    //[SerializeField] private bool commandStackDebug = false;
    [SerializeField] private string[] commandStack;
    [SerializeField] private string[] undoStack;
    private void FillCommandStacks()
    {
        commandStack = WorldStateManager.Instance.GetCommandStackAsArray();
        undoStack = WorldStateManager.Instance.GetUndoStackAsArray();
    }

    [Header("World Statistics")]
    //[SerializeField] private bool worldStatisticsDebug = false;
    [SerializeField] private int totalWorldObjects;
#endif
}
