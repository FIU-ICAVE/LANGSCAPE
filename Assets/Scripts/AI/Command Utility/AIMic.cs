using UnityEngine;
using Meta.WitAi.TTS.Utilities;

class AIMic : MonoBehaviour
{

    private static AIMic _instance;
    public static AIMic Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AIMic::Instance - AIMic is null");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Variables
    [SerializeField] private TTSSpeaker speaker;

    // Message Must not be Empty
    public void SpeakFluff(string message)
    {
        // Variable
        string input;
            
        // If Message Declaration is Empty, input equals No Text Response
        if (string.IsNullOrEmpty(message)) { input = "No Text Response Detected"; }
        // Else, input equals message
        else { input = message; }

        // Speak Input
        speaker.Speak(input);
            
    }
    //It Exists
    public void ShushFluff()
    {
        // Silence Speaker
        speaker.Stop();
    }
}