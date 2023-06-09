using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    /*
        Singleton
    */
    private static DataPersistenceManager _instance;
    public static DataPersistenceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("DataPersistenceManager::Instance - DataPersistenceManager is null");
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    private GameData _gameData;

    private void Start()
    {
        LoadGame();
    }
    public void NewGame()
    {
        this._gameData = new GameData();
    }
    public void LoadGame()
    {
        //[[[TODO - load any saved date from a file using the file data handler]]]

        //if no data to be loaded, initialize a new game (default values)
        if(this._gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            NewGame();
        }

        //[[[TODO - push the loaded data to all other scripts that need it]]]
    }
    public void SaveGame()
    {
        //[[[TODO - pass the data to other scripts so they can update it]]]

        //[[[TODO - save the data to a file using file data handler]]]
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
