using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LangscapeError {
    /*
        Constructor
    */
    public int code { get; private set; }
    public string message;
    public LangscapeError(int code, string messageToUser) {
        this.code = code;
        this.message = messageToUser;
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
    //public static LangscapeError CMD_INVALID                      = new LangscapeError(11001, "That is not a valid command. ");
    public static LangscapeError CMD_INVALID_PARAM = new LangscapeError(11002, "That value is invalid. Please try another value.");
    public static LangscapeError CMD_INVALID_PARAMS = new LangscapeError(11003, "One of those values is invalid. Please try another value.");
    public static LangscapeError CMD_POSITION_OUT_OF_WORLD = new LangscapeError(11004, "That position is out of bounds.");
    public static LangscapeError CMD_DESTINATION_OUT_OF_WORLD = new LangscapeError(11005, "The new destination would be out of bounds.");
    public static LangscapeError CMD_ROTATION_INVALID = new LangscapeError(11006, "That rotation is invalid. Rotations must be greater than zero and less than two-hundred and seventy.");

    //LLM errors
    public static LangscapeError LLM_UNAVAILABLE = new LangscapeError(20000, "Sorry, the LLM is not available to respond right now.");
    public static LangscapeError LLM_INVALID_RESPONSE = new LangscapeError(20001, "Sorry, the LLM did not understand. Could you repeat or rephrase?"); //LLM doesn't respond correctly

    public static bool operator ==(int left, LangscapeError right) => left == right.code;
    public static bool operator ==(LangscapeError left, int right) => left.code == right;
    public static bool operator !=(int left, LangscapeError right) => left != right.code;
    public static bool operator !=(LangscapeError left, int right) => left.code != right;

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
