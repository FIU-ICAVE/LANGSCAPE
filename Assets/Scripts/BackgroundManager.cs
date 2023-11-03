using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using static BackgroundParser;
using ObjectTracker;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    // Variables
    const int MaxObjects = 50;
    ToyBox box;
    List<GameObject> items;

    [Header("Background Scripts")]
    //<Don't forget to declare them public>
    /* Script for Changing Skybox */
    public SkyChanger skyChanger;
    /* Script for Changing Land */
    //public LandChanger landChanger;
    /* Script for Spawn and Despawning Objects */
    public Spawner spawner;

    [Header("Object Prefabs")]
    // <Should ItemPrefabs Be Held Here or in each of the functions?>
    public GameObject[] itemPrefabs = new GameObject[5];
    // For Keeping Track of Objects
    

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

        // Keeps Track of Objects in World for Object and Utility Commands
        box = new ToyBox();
        items = new List<GameObject> ();

    }


    /* Manipulate Sky */
    public void d_Change(int state)
    {
        // <Anything else to Add?>
        skyChanger.ChangeSky(state);
    }

    /* Manipulate Land*/
    public void l_Change(int area)
    {
        // Add LandChanger
    }

    /* Manipulate Objects */
    public int ObjectSpawn(int item)
    {
        if(items.Count > MaxObjects)
        {
            //FOR TOO MANY OBJECTS DESPAWN FIRST ITEM?
            var res = box.RetrieveAddedInfoByPosition(0);
            int error = DestroyObject(res.objectId);
            if (error != LangscapeError.CMD_VALID.code)
            {
                return error;
            }
        }
        if(item > itemPrefabs.Length)
        {
            // ERROR CODE FOR NONEXISTENT OBJECT
            return LangscapeError.CMD_OBJECT_NA.code;
        }
        // Keep Track
        int id = box.CreateObject(item);

        // Spawn and Name
        GameObject Toy = spawner.Spawn(itemPrefabs[item]);
        Toy.name = "item_" + id.ToString();

        // Keep Track Part 2
        items.Add(Toy);

        return LangscapeError.CMD_VALID.code;
    }

    // Utilities
    public int Utility(int util, int opt = 0)
    {
        int error = LangscapeError.CMD_VALID.code;

        switch (util)
        {
            case 1:
                // Placeholder
                break;
            case 2:
                // Placeholder
                break;
            default:
                //
                break;
        }

        return error;
    }

    public int DestroyObject(int item, int opt = 1)
    {
        int pos = box.RemoveObjectByObjectId(item, opt);
        if(pos > -1)
        {
            spawner.Despawn(items[pos]);
            items.RemoveAt(pos);
            return LangscapeError.CMD_VALID.code;
        }
        else
        {
            //ERROR FOR NONEXISTENT GameObject
            return LangscapeError.CMD_OBJECT_D_NA.code;
        }
        
    }
    
    public void UndoPreviousCommand(int item)
    {

    }


    /* Execute Everything */
    public void Execute(string response)
    {
        // Declare Default ERROR Code
        int error = LangscapeError.CMD_VALID.code;
        // Grab Parsed Command List
        List<BackgroundParser.b_command> b_coms = BackgroundParser.Parse(response);

        // Check if Empty
        if(b_coms.Count > 0)
        {
            // 
            for(int i = 0; i < b_coms.Count; i++)
            {
                BackgroundParser.b_command bcom = b_coms[i];

                // Leave it Error Sensed
                if(bcom.errorCode != LangscapeError.CMD_VALID.code)
                {
                    error = bcom.errorCode;
                    break;
                }

                // Checks Command Type
                switch (bcom.c_ind)
                {
                    case 0:
                        // No Change
                        break;
                    case 1:
                        d_Change(bcom.act);
                        break;
                    case 2:
                        // Land
                        break;
                    case 3:
                        error = ObjectSpawn(bcom.act);
                        break;
                    case 4:
                        if (bcom.extra)
                        {
                            // Utility with Two Params
                            error = Utility(bcom.act, bcom.act2);
                        }
                        else
                        {
                            // Utility With One Param
                            error = Utility(bcom.act);
                        }

                        break;
                }

                if(error != LangscapeError.CMD_VALID.code)
                {
                    break;
                }

            }
        }
        else
        {
            error = LangscapeError.CMD_NULL.code;
        }
        
        // Shush Second Speaker if Error Occurs
        if (error != LangscapeError.CMD_VALID.code)
        {
            AIMic.Instance.ShushFluff();
        }
        
        // Summon Langscape Error
        LangscapeError.Instance.ThrowUserError(error);
        
    }

}