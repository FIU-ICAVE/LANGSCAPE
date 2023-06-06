/*
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
        _instance = this;
    }

    /*
        Adding or removing objects
    */
    private Dictionary<Vector3, GameObject> worldObjectData = new Dictionary<Vector3, GameObject>();

    public void AddWorldObject(GameObject newObj)
    {
        Vector3 newObjPos = newObj.transform.position;

        if (newObj == null)
        {
            //[[[ERROR MESSAGE BACK TO AI]]]
            Debug.LogError("WorldStateManager::AddWorldObject - The new object is null");
        }
        if (!worldObjectData.TryGetValue(newObj.transform.position, out GameObject o))
        {
            //[[[ERROR MESSAGE BACK TO AI]]]
            //[[[inform user that position was occupied]]]
            Debug.LogError("WorldStateManager::AddWorldObject - There is already an object there!");
        }

        worldObjectData.Add(newObjPos, newObj);
        //[[[add to undo/redo stack]]]
    }
    public void RemoveWorldObject(GameObject objToRemove)
    {
        if (!worldObjectData.TryGetValue(objToRemove.transform.position, out GameObject o))
        {
            Debug.LogError("WorldStateManager::RemoveWorldObject - That object was not found.");
        }

        worldObjectData.Remove(objToRemove.transform.position);
    }
    public void RemoveWorldObject(Vector3 posToRemove)
    {
        if (!worldObjectData.TryGetValue(posToRemove, out GameObject o))
        {
            Debug.LogError("WorldStateManager::RemoveWorldObject - That object was not found.");
        }

        worldObjectData.Remove(posToRemove);
    }
    /*
        ...further RemoveWorldObject's
        ideas:
            - color
            - material
            - custom model type (requires further implementation)
     */

    /*
        Command stack
    */
    private Stack<string> commandStack = new Stack<string>();
    public void ExecutePush(Command cmd)
    {
        cmd.Execute();
        commandStack.Push(cmd.ToString());
    }
    public void UndoPop(Command cmd)
    {
        cmd.Undo();
        commandStack.Pop();
    }

    /*
        World statistics
    */
    public int GetTotalWorldObjects()
    {
        return worldObjectData.Count;
    }
    public int GetWorldObjOfMaterial(Material mat)
    {
        //to do
        //[[[how does this work with the single mesh implementation?]]]
        return 0;
    }

    /*
        Utility methods
    */
    public bool PositionOccupied(Vector3 pos)
    {
        foreach(Vector3 currentPositions in worldObjectData.Keys)
        {
            if(pos == currentPositions)
            {
                return true;
            }
        }

        return false;
    }
}
