using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ObjectTracker;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    // Variables
    const int MaxObjects = 50;
    ToyBox box = new ToyBox();

    [Header("Background Scripts")]
    //<Don't forget to declare them public>
    /* Script for Changing Skybox */
    //public SkyChanger sky;
    /* Script for Changing Land */
    //public LandChanger land;
    /* Script for Spawn and Despawning Objects */
    public Spawner sp;

    [Header("Object Prefabs")]
    // <Should ItemPrefabs Be Held Here or in each of the functions>
    public GameObject[] itemPrefabs = new GameObject[5];

    /*
        Singleton
    */
    private static BackgroundManager _instance;
    public static BackgroundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("BackgroundManager::Instance - BackgroundManager is null");
            }

            return _instance;
        }
    }


    private void Awake()
    {
        // Singleton
        _instance = this;

    }
    

    // Manipulate Sky

    // Manipulate Land

    // Manipulate Objects

    // Keeps Track of Object Count in World for Object and Utility Commands

}