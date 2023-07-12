/*
    TO DO:
        - ThrowErrorToUser
*/
using UnityEngine;
using Meta.WitAi.TTS.Samples;

public class ErrorInterface : MonoBehaviour {
    /*
        Singleton
    */
    private static ErrorInterface _instance;
    public static ErrorInterface Instance {
        get {
            if (_instance == null) {
                Debug.LogError("LangscapeError::Instance - LangscapeError is null");
            }

            return _instance;
        }
    }

    [SerializeField] private TTSSpeakerInput _speakerInput;

    private void Awake() {
        _instance = this;
    }

    public void ThrowUserError(LangscapeError e) {
        _speakerInput.input = e.message;
        _speakerInput.SpeakClick();
    }

    public void ThrowUserError(int code) {
        //developer utility errors
        if (code == LangscapeError.CMD_VALID) {
            //Debug.LogWarning(CMD_VALID.message);
            return;
        }
        if (code == LangscapeError.CMD_UNINITIALIZED) {
            //Debug.LogWarning(CMD_UNINITIALIZED.message);
            return;
        }

        //command errors
        if (code == LangscapeError.CMD_NULL) {
            ThrowUserError(LangscapeError.CMD_NULL);
            return;
        }
        if (code == LangscapeError.CMD_INVALID_PARAM) {
            ThrowUserError(LangscapeError.CMD_INVALID_PARAM);
            return;
        }
        if (code == LangscapeError.CMD_INVALID_PARAM) {
            ThrowUserError(LangscapeError.CMD_INVALID_PARAM);
            return;
        }
        if (code == LangscapeError.CMD_POSITION_OUT_OF_WORLD) {
            ThrowUserError(LangscapeError.CMD_POSITION_OUT_OF_WORLD);
            return;
        }
        if (code == LangscapeError.CMD_DESTINATION_OUT_OF_WORLD) {
            ThrowUserError(LangscapeError.CMD_DESTINATION_OUT_OF_WORLD);
            return;
        }
        if (code == LangscapeError.CMD_ROTATION_INVALID) {
            ThrowUserError(LangscapeError.CMD_ROTATION_INVALID);
            return;
        }

        //LLM errors
        if(code == LangscapeError.LLM_UNAVAILABLE) {
            ThrowUserError(LangscapeError.LLM_UNAVAILABLE);
            return;
        }
        if (code == LangscapeError.LLM_INVALID_RESPONSE) {
            ThrowUserError(LangscapeError.LLM_INVALID_RESPONSE);
            return;
        }
    }

}