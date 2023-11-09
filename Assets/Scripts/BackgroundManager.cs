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
    /* For Keeping Track of Current Environments */
    struct environ
    {
        public int sky;
        public int land;

        public environ(int sCode, int lCode)
        {
            this.sky = sCode;
            this.land = lCode;
        }
    }
    List<environ> e_history;

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
        e_history = new List<environ>();

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
        // <Anything else to Add?>
        groundChanger.changeGround(area);
    }

    /* Manipulate Objects */
    public int ObjectSpawn(int item2)
    {
        int item = item2 - 1;

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

    /* Utilities */
    // Main Hub for All Utility Matters :: Declutters Execute() ::
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
    // Undo Previous Command
    public int UndoPreviousCommand()
    {
        int error = LangscapeError.CMD_VALID.code;
        // Check if there is at least one executed command in History
        if(history.Count > 0)
        {
            // Pull 2nd to Last Command From History
            BackgroundParser.b_command snip = history[history.Count - 1];
            // Do Nothing
            if(snip.c_ind == 0)
            {
                snip = new BackgroundParser.b_command(error);
                // Do Nothing
            }
            // Revert Sky
            else if(snip.c_ind == 1)
            {
                environ s = e_history[e_history.Count - 1];
                snip = new BackgroundParser.b_command(error, 1, s.sky, false);
            }
            // Revert Land
            else if(snip.c_ind == 2)
            {
                environ s = e_history[e_history.Count -1];
                snip = new BackgroundParser.b_command(error, 2, s.land, false);
            }
            // Revert Object Spawn
            else if(snip.c_ind == 3)
            {
                // Utility >> DestroyObject() >> Position: Last
                snip = new BackgroundParser.b_command(error, 4, /*Placeholder*/ 2, true, box.GetAddedCount() - 1);


            }
            // Revert Utility Change
            else if (snip.c_ind == 4)
            {
                snip = new BackgroundParser.b_command(error);
            }
            
            
            error = CommandExecutor(snip, error);
        }
        return error;
    }
    // Redo Previous Command
    public int RedoPreviousCommand()
    {
        int error = LangscapeError.CMD_VALID.code;
        // Check if there is at least one executed command in History
        if (history.Count > 0)
        {
            // Pull Last Command From History
            BackgroundParser.b_command snip = history[history.Count - 1];
            error = CommandExecutor(snip, error);
        }
        return error;
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
                // Grab Current Environment and Store
                environ current = new environ(skyChanger.currentSky(), groundChanger.currentGround());
                e_history.Add(current);

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