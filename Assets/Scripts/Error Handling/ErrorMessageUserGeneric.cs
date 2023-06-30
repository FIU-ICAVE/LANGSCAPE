using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorMessageUserGeneric
{
    public static Dictionary<string, string> genericMessages = new Dictionary<string, string>();

    private void Awake()
    {
        //add new generic error messages here
        genericMessages.Add("ERROR_POSITION_OUT_OF_WORLD", "That position is out of bounds!");
    }
}
