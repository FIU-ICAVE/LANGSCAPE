using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using static BackgroundParser;
using ObjectTracker;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    // Variables
    /* For Keeping Track of Objects */
    const int MaxObjects = 50;
    ToyBox box;
    List<GameObject> items;
    /* For Keeping Track of Commands */
    List<BackgroundParser.b_command> history;

    [Header("Background Scripts")]
    //<Don't forget to declare them public>
    /* Script for Changing Skybox */
    public SkyChanger skyChanger;
    /* Script for Changing Land */
    public GroundChanger groundChanger;
    /* Script for Spawn and Despawning Objects */
    public Spawner spawner;

    [Header("Object Prefabs")]
    // <Should ItemPrefabs Be Held Here or in each of the functions?>
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

        // Keeps Track of Objects in World for Object and Utility Commands
        box = new ToyBox();
        items = new List<GameObject>();
        history = new List<BackgroundParser.b_command>();

    }


    /* Manipulate Sky */
    public void d_Change(int state)
    {
        skyChanger.ChangeSky(state);
    }

    /* Manipulate Land*/
    public void l_Change(int area)
    {
        groundChanger.changeGround(area);
    }

    /* Manipulate Objects */
    public int ObjectSpawn(int item2)
    {
        int item = item2 - 1;

        if(items.Count > MaxObjects)
        {
            //FOR TOO MANY OBJECTS DESPAWN FIRST ITEM
            var res = box.RetrieveAddedInfoByPosition(0);
            int error = DestroyObject(res.objectId);
            if (error != LangscapeError.CMD_VALID.code)
            {
                return error;
            }
        }
        if(item >= itemPrefabs.Length)
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

    /* Utilities */
    // Main Hub for All Utility Matters :: Declutters Execute() ::
    public int Utility(int util, int opt = 0)
    {
        int error = LangscapeError.CMD_VALID.code;
        int count = 0;
        string floof = string.Empty;

        switch (util)
        {
            case 1:
                // Remove Object
                error = DestroyObject(opt - 1);
                break;
            case 2:
                // Count By object_num
                count = box.GetAddedObjectCount(opt - 1);
                if (count == 1)
                {
                    floof = string.Format("There is currently {0} Spawned in the world.", count);
                }
                else
                {
                    floof = string.Format("There are currently {0} Spawned in the world.", count);
                }
                AIMic.Instance.SpeakFluff(floof);
                break;
            case 3:
                // Count all Objects
                count = box.GetAddedCount();
                if (count == 1) {
                    floof = string.Format("There is currently {0} Spawned Object in the world.", count);
                }
                else
                {
                    floof = string.Format("There are currently {0} Spawned Objects in the world.", count);
                }
                AIMic.Instance.SpeakFluff(floof);
                break;
            case 4:
                // Destroy All of A Specific Object
                error = DestroyAllByObject(opt-1);
                break;
            case 5:
                // Destroy All Spawned Objects
                error = DestroyAllObjects();
                break;
            default:
                error = LangscapeError.CMD_INVALID_PARAM.code;
                break;
        }

        return error;
    }
    // Deletes All Instances of Object[item]
    public int DestroyAllByObject(int item)
    {
        int error = LangscapeError.CMD_VALID.code;
        int count = box.GetAddedObjectCount(item);

        if (count < 1)
        {
            // Error for Zero Count
            return LangscapeError.CMD_OBJECT_D_NA.code;
        }
        // Clear List of Specific Objects [item]
        for(int i = 0; i < count; i++)
        {
            error = DestroyObject(item);
            if(error != LangscapeError.CMD_VALID.code)
            {
                return error;
            }
        }
        // Clear Box of Specific Objects [item]
        box.RemoveAllObjectsByObjectId(item);
        
        return error;
    }
    //Deletes All Objects
    public int DestroyAllObjects()
    {
        int error = LangscapeError.CMD_VALID.code;

        if (box.GetAddedCount() < 1)
        {
            // Error for Zero Count
            return LangscapeError.CMD_OBJECT_D_NA.code;
        }
        // Despawn Items
        for(int i = items.Count - 1; i >= 0; i--)
        {
            spawner.Despawn(items[i]);
        }
        // Clear List and Box
        items.Clear();
        box.RemoveAllObjects();
        
        return error;
    }
    // Deletes Specific Nth[opt] Instance of Object[item]
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

                // Method for Initiating Type
                error = CommandExecutor(bcom, error);
                
                // Error Catcher
                if(error != LangscapeError.CMD_VALID.code)
                {
                    break;
                }
                // Add to Command History
                history.Add(bcom);
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
    // <Ask about Doing Undo if N Subsists>
    public int CommandExecutor(BackgroundParser.b_command bcom, int errno)
    {
        // Variables
        int error = errno;

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
                l_Change(bcom.act);
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

        return error;
    }

}