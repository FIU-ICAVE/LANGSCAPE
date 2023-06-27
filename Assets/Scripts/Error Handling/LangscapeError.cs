/*
    TO DO:
        - ThrowErrorToUser
*/
using Meta.WitAi.TTS.Samples;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
                UnityEngine.Debug.LogError("LangscapeError::Instance - LangscapeError is null");
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    /*
        Fields
    */
    [SerializeField] private TTSSpeakerInput _speakerInput;

    /*
        Constructor
    */
    private int code;
    private string message;
    public LangscapeError(int code, string messageToUser)
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
        message = messageToUser;
    }

    /*
        Errors
        
        Format:
            - Unity errors: 1XXXX
                - command errors: 11XXX

            - LLM errors: 2XXXX
    */
    //command errors
    public static LangscapeError CMD_NULL                       = new LangscapeError(11000, "Sorry, I didn't understand that. Could you repeat or rephrase?");
    public static LangscapeError CMD_INVALID                    = new LangscapeError(11001, "That is not a valid command. Please rephrase or try something else.");
    public static LangscapeError CMD_INVALID_PARAM              = new LangscapeError(11002, "That value is invalid. Please try another value.");
    public static LangscapeError CMD_INVALID_PARAMS             = new LangscapeError(11002, "One of those values is invalid. Please try another value.");
    public static LangscapeError CMD_POSITION_OUT_OF_WORLD      = new LangscapeError(11003, "That position is out of bounds.");
    public static LangscapeError CMD_DESTINATION_OUT_OF_WORLD   = new LangscapeError(11004, "The new destination would be out of bounds.");
    public static LangscapeError CMD_ROTATION_INVALID           = new LangscapeError(11005, "That rotation is invalid. Rotations must be greater than zero and less than two-hundred and seventy.");

    //LLM errors
    public static LangscapeError LLM_INVALID_RESPONSE           = new LangscapeError(20000, "Sorry, the LLM did not understand. Could you repeat or rephrase?");

    /*
        Methods
    */
    public void ThrowUserError(LangscapeError e)
    {
        _speakerInput.input = e.message;
        _speakerInput.SpeakClick();
    }
}
