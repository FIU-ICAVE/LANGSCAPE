/*
    TO DO:
        - ThrowErrorToUser
*/
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Tutorials.Core.Editor;
using UnityEngine;

public class LangscapeError : MonoBehaviour
{
    /*
        Singleton
    */
    private static LangscapeError _instance;
    public static LangscapeError Instance
    {
        get
        {
            if(_instance == null)
            {
                UnityEngine.Debug.LogError("LangscapeError::Instance - LangscapeManager is null");
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    /*
        Constructor
    */
    private int code;
    private string message;
    private LangscapeError(int code, string messageToUser)
    {
        StackFrame callerClass = new StackTrace().GetFrame(1);
        MethodBase callerMethod = callerClass.GetMethod();
        if(code < 0)
        {
            UnityEngine.Debug.LogError(callerClass.GetFileName().ToString() + "::" + callerMethod.Name.ToString() + " - Error code cannot be less than 0.");
        }
        if(messageToUser.IsNullOrWhiteSpace())
        {
            UnityEngine.Debug.LogError(callerClass.GetFileName().ToString() + "::" + callerMethod.Name.ToString() + " - Error message cannot be empty or ull");
        }
        
        this.code = code;
        this.message = messageToUser;
    }

    /*
        Errors
        
        Format:
            - Unity errors: 1XXXX
                - command errors: second digit = 11XXX

            - LLM errors: 2XXXX
    */
    //command errors
    public static LangscapeError ERROR_POSITION_OUT_OF_WORLD = new LangscapeError(11001, ErrorMessageUserGeneric.genericMessages["ERROR_POSITION_OUT_OF_WORLD"]);

    /*
        Methods
    */
    public void ThrowErrorToUser(LangscapeError e)
    {
        //[[[TO DO]]]
        //call whisper to do text-to-speech
    }
}
