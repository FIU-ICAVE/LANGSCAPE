/*
    Authors: Jose Gonzalez Lopez, Ian Rodriguez
    Script Description:
        This script serves as the entry point for the command system. It handles converting voice 
        input into a valid JSON object.
*/
using Meta.WitAi;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using Oculus.Voice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OpenAI;
using UnityEngine.UI;

public class VoiceToCommand : MonoBehaviour {
    // UI
    [SerializeField, Multiline] private string inputBoxDefaultText;
    [SerializeField] private TMP_Text inputBox;
    [SerializeField] private TMP_Text outputBox;
    [SerializeField] private Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;
    [Header("Voice")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [Header("OVR")]
    [SerializeField] private GestureTest gesture;

    // OpenAI Settings
    private List<ChatMessage> messages = new List<ChatMessage>();
    private OpenAIApi openai;
    private string prompt;

    // Whether voice is activated
    public bool IsActive => _active;
    private bool _active = false;

    void Awake() {
        //setting OpenAI API key
        openai = new OpenAIApi(apiKey: "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");

        //loading prompt
        TextAsset filedata = Resources.Load<TextAsset>("OpenAI/PROMPT");
        if (filedata == null)
            throw new System.Exception("No file found called prompt in 'Assets/Resources/OpenAI/PROMPT");
        prompt = filedata.text;
        Debug.Log(prompt);
    }

    // Add delegates
    private void OnEnable() {
        inputBox.text = inputBoxDefaultText;
        appVoiceExperience.VoiceEvents.OnSend.AddListener(OnSend);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnStartListening.AddListener(OnListenStart);
        appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnListenStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.AddListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.AddListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnRequestResponse);
        appVoiceExperience.VoiceEvents.OnError.AddListener(OnRequestError);
    }
    // Remove delegates
    private void OnDisable() {
        appVoiceExperience.VoiceEvents.OnSend.RemoveListener(OnSend);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnStartListening.RemoveListener(OnListenStart);
        appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(OnListenStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.RemoveListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.RemoveListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnRequestResponse);
        appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnRequestError);
    }

    private void Start()
    {
        //microphone dropdown
        dropdown.ClearOptions();
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        dropdown.onValueChanged.AddListener(ChangeMicrophone);
        inputBox.text = inputBoxDefaultText;
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();

        messages.Add(new ChatMessage() { Role = "system", Content = prompt });
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    // STT request began
    private void OnSend(VoiceServiceRequest request) {
        _active = true;
    }
    // Request transcript
    private void OnRequestTranscript(string transcript) {
        inputBox.text = transcript;
    }
    // Listen start
    private void OnListenStart()
    {
        inputBox.text = "Listening...";
        symbol.enabled = true;
    }
    // Listen stop
    private void OnListenStop()
    {
        inputBox.text = "Processing...";
        symbol.enabled = false;
    }
    // Listen stop
    private void OnListenForcedStop()
    {
        inputBox.text = inputBoxDefaultText;
        symbol.enabled = false;
        OnRequestComplete();
    }
    // Request response
    private void OnRequestResponse(WitResponseNode response)
    {
        if (!string.IsNullOrEmpty(response["text"]))
        {
            inputBox.text = response["text"];
            ToGPT(inputBox.text);
        }
        else
        {
            inputBox.text = inputBoxDefaultText;
        }
        OnRequestComplete();
    }
    // Request error
    private void OnRequestError(string error, string message)
    {
        OnRequestComplete();
    }
    // Deactivate
    private void OnRequestComplete()
    {
        _active = false;
    }
    // Set activation
    public void SetActivation(bool state)
    {
        if (_active != state) {
            _active = state;
            if (_active)
                appVoiceExperience.Activate();
            else
                appVoiceExperience.Deactivate();
        }
    }

    private void Update() {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.V))
            SetActivation(true);
        if (Input.GetKeyUp(KeyCode.V))
            SetActivation(false);
#else
        if (gesture.selected)
            SetActivation(true);
        if (!gesture.selected)
            SetActivation(false);
#endif
    }

    private async void ToGPT(string request) {
        ChatMessage userRequest = new ChatMessage() {
            Role = "user",
            Content = request
        };

        messages.Add(userRequest);

        outputBox.text = "Processing...";

        // Complete the instruction
        try {
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest() {
                Model = "gpt-3.5-turbo-16k",
                Messages = messages,
                Temperature = 0f,
                MaxTokens = 256,
                PresencePenalty = 0,
                FrequencyPenalty = 0
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0) {
                var aiResponse = completionResponse.Choices[0].Message;
                aiResponse.Content = aiResponse.Content.Trim();

                messages.Add(aiResponse);
                outputBox.text = aiResponse.Content; //displaying LLM response to UI

                //Debug.Log("CommandInterpreter message: " + aiResponse.Content); //[[[CONSIDER MOVING THIS TO DEBUG SUITE]]]
                WorldStateManager.Instance.BuildCommandBatch(aiResponse.Content, userRequest.Content);
            } else {
                outputBox.text = LangscapeError.LLM_UNAVAILABLE.message;
            }
        } catch (System.Exception e) {
            outputBox.text = e.Message;
        }
    }
}
