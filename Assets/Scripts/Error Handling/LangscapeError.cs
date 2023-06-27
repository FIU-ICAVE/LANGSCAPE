/*
    TO DO:
        - ThrowErrorToUser
*/
using UnityEngine;
using Meta.WitAi.TTS.Samples;

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
            if (_instance == null)
            {
                Debug.LogError("LangscapeError::Instance - LangscapeError is null");
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
    public int code { get; private set; }
    private string message;
    public LangscapeError(int code, string messageToUser)
    {
        /*
        StackFrame callerClass = new StackTrace().GetFrame(1);
        MethodBase callerMethod = callerClass.GetMethod();
        if(code < 0)
        {
            UnityEngine.Debug.LogError(callerClass.GetFileName().ToString() + "::" + callerMethod.Name.ToString() + " - Error code cannot be less than 0.");
        }
        if(messageToUser.IsNullOrWhiteSpace())
        {
            UnityEngine.Debug.LogError(callerClass.GetFileName().ToString() + "::" + callerMethod.Name.ToString() + " - Error message cannot be empty or null");
        }
        */

        this.code = code;
        message = messageToUser;
    }

    /*
        Errors
        
        Format:
            - Developer Utility errors (usually won't be read out to user): 0XXXX

            - Unity errors: 1XXXX
                - command errors: 11XXX

            - LLM errors: 2XXXX
    */
    //developer utility errors
    public static LangscapeError CMD_VALID = new LangscapeError(00000, "Valid");
    public static LangscapeError CMD_UNINITIALIZED = new LangscapeError(00001, "Uninitialized");

    //command errors
    public static LangscapeError CMD_NULL = new LangscapeError(11000, "Sorry, I didn't understand that. Please rephrase or try something else."); //user says something unrelated to app
    //public static LangscapeError CMD_INVALID                    = new LangscapeError(11001, "That is not a valid command. ");
    public static LangscapeError CMD_INVALID_PARAM = new LangscapeError(11002, "That value is invalid. Please try another value.");
    public static LangscapeError CMD_INVALID_PARAMS = new LangscapeError(11003, "One of those values is invalid. Please try another value.");
    public static LangscapeError CMD_POSITION_OUT_OF_WORLD = new LangscapeError(11004, "That position is out of bounds.");
    public static LangscapeError CMD_DESTINATION_OUT_OF_WORLD = new LangscapeError(11005, "The new destination would be out of bounds.");
    public static LangscapeError CMD_ROTATION_INVALID = new LangscapeError(11006, "That rotation is invalid. Rotations must be greater than zero and less than two-hundred and seventy.");

    //LLM errors
    public static LangscapeError LLM_INVALID_RESPONSE = new LangscapeError(20000, "Sorry, the LLM did not understand. Could you repeat or rephrase?"); //LLM doesn't respond correctly

    /*
        Methods
    */
    public void ThrowUserError(LangscapeError e)
    {
        _speakerInput.input = e.message;
        _speakerInput.SpeakClick();
    }
    public void ThrowUserError(int code)
    {
        //developer utility errors
        if (code == CMD_VALID)
        {
            Debug.LogWarning(CMD_VALID.message);
            return;
        }
        if (code == CMD_UNINITIALIZED)
        {
            Debug.LogWarning(CMD_UNINITIALIZED.message);
            return;
        }

        //command errors
        if (code == CMD_NULL)
        {
            ThrowUserError(CMD_NULL);
            return;
        }
        if (code == CMD_INVALID_PARAM)
        {
            ThrowUserError(CMD_INVALID_PARAM);
            return;
        }
        if (code == CMD_INVALID_PARAM)
        {
            ThrowUserError(CMD_INVALID_PARAM);
            return;
        }
        if (code == CMD_POSITION_OUT_OF_WORLD)
        {
            ThrowUserError(CMD_POSITION_OUT_OF_WORLD);
            return;
        }
        if (code == CMD_DESTINATION_OUT_OF_WORLD)
        {
            ThrowUserError(CMD_DESTINATION_OUT_OF_WORLD);
            return;
        }
        if (code == CMD_ROTATION_INVALID)
        {
            ThrowUserError(CMD_ROTATION_INVALID);
            return;
        }

        //LLM errors
        if (code == LLM_INVALID_RESPONSE)
        {
            ThrowUserError(LLM_INVALID_RESPONSE);
            return;
        }
    }

    public static bool operator == (int left, LangscapeError right) => left == right.code;
    public static bool operator != (int left, LangscapeError right) => left != right.code;
}