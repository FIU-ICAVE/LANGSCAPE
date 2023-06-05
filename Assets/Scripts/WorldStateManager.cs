/*
    NOTES:
        - consider making a GameManager to Init properties in this class
    TO DO:
        - (maybe) implement some sort of threshold for checking if new pos is already taken
        - allow AI to query WorldState properties
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
    public int NumWorldObjects { get; set; }
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
        NumWorldObjects++;
    }
    public void RemoveWorldObject(GameObject objToRemove)
    {
        if (!worldObjectData.TryGetValue(objToRemove.transform.position, out GameObject o))
        {
            Debug.LogError("WorldStateManager::RemoveWorldObject - That object was not found.");
        }

        worldObjectData.Remove(objToRemove.transform.position);
        NumWorldObjects--;
    }
    public void RemoveWorldObject(Vector3 posToRemove)
    {
        if (!worldObjectData.TryGetValue(posToRemove, out GameObject o))
        {
            Debug.LogError("WorldStateManager::RemoveWorldObject - That object was not found.");
        }

        worldObjectData.Remove(posToRemove);
        NumWorldObjects--;
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
