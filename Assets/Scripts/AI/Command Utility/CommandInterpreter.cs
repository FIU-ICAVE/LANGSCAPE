/*
 * Author: Christian Laverde, Justin Cardoso, Ian Rodriguez
 * Script Description:
 *      Handles converting voice input into valid JSON object.
 * NOTES:
 *      - consider moving Debug.Log() that outputs the command to DebugSuite
 * TO DO:
 *      - Interpret JSON string to valid JSON
 *      - Expand on the instructions for the prompt
 *      - Enable speech recognition via OVR
 */

using Meta.WitAi;
using Oculus.Voice.Demo;
using OpenAI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Rendering.DebugUI;
#if !UNITY_STANDALONE_WIN
using Oculus.Interaction;
//using UnityEngine.Windows.Speech;
#endif

public class CommandInterpreter : MonoBehaviour {
    // UI
    [SerializeField] private UnityEngine.UI.Text inputBox;
    [SerializeField, Multiline] private string inputBoxDefaultText;
    //[SerializeField] private TMP_Text inputBox;
    [SerializeField] private UnityEngine.UI.Text outputBox;
    //[SerializeField] private TMP_Text outputBox;
    [SerializeField] private UnityEngine.UI.Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;

    // OpenAI Settings
    private OpenAIApi openai;
    private string prompt;

    // Whisper
    private readonly string fileName = "output.wav";
    /*
    private readonly int MAX_DURATION = 30;
    private AudioClip clip;
    private bool isRecording;
    private float time = 0;
    */

    // Wit STT
    [SerializeField] private InteractionHandler interactionHandler;
    private bool isRecording = false;
    private float time = 0;
    private readonly int MAX_DURATION = 30;

    //Gesture Detect
    public GestureTest gesture;

    // ChatGPT
    private List<ChatMessage> messages = new List<ChatMessage>();

    void Awake() 
    {
        //setting OpenAI API key
        openai = new OpenAIApi(apiKey: "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");
        
        //loading prompt
        TextAsset filedata = Resources.Load<TextAsset>("OpenAI/PROMPT");
        if (filedata == null)
            throw new System.Exception("No file found called prompt in 'Assets/Resources/OpenAI/PROMPT");
        prompt = filedata.text;
        Debug.Log(prompt); //[[[ADD TO DEBUG SUITE]]]
    }

    private void Start() 
    {
        //microphone dropdown
        dropdown.ClearOptions();
        foreach (var device in Microphone.devices) {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        dropdown.onValueChanged.AddListener(ChangeMicrophone);
        inputBox.text = inputBoxDefaultText;
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();
        
        messages.Add(new ChatMessage() { Role = "system", Content = prompt });
    }

    private void ChangeMicrophone(int index) {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    /*private void StartRecording() {
        isRecording = true;
        symbol.enabled = true;
        inputBox.text = "Listening...";

#if UNITY_STANDALONE_WIN
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        clip = Microphone.Start(dropdown.options[index].text, false, MAX_DURATION, 44100);
#else
        //var index = PlayerPrefs.GetInt("user-mic-device-index");
        clip = Microphone.Start(dropdown.options[0].text, false, MAX_DURATION, 44100);
#endif
    }
    private async void EndRecording() {
        isRecording = false;
        symbol.enabled = false;
        time = 0;
        inputBox.text = "Transcribing...";
        Microphone.End(null);
        byte[] data = SaveWav.Save(fileName, clip);
        var req = new CreateAudioTranscriptionsRequest {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            Model = "whisper-1",
            Language = "en"
        };
        var res = await openai.CreateAudioTranscription(req);
        inputBox.text = res.Text;
        if (res.Error != null)
            inputBox.text = "You wont believe it.";
        if (res.Text != "")
            CreateJSON(res.Text);
    }*/

    private void StartRecording2()
    {
        //start listening
        interactionHandler.SetActivation(true);
        isRecording = true;

        //update UI
        symbol.enabled = true;
        inputBox.text = "Listening...";
    }

    private async void StopRecording2()
    {
        //stop listening
        Debug.Log("before: " + inputBox.text);
        interactionHandler.SetActivation(false);
        Debug.Log("after: " + inputBox.text);
        isRecording = false;

        //update UI
        symbol.enabled = false;
        //inputBox.text = "Transcribing...";

        //send input text to LLM for transcription & update UI again
        Debug.Log("after2: " + inputBox.text);
        CreateJSON(inputBox.text);
        //inputBox.text = inputBoxDefaultText;
    }

    // Update is called once per frame
    void Update() {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartRecording2();
            //StartRecording();
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            StopRecording2();
            //EndRecording();
        }
#else
        if (!isRecording && gesture.selected)
            StartRecording();
        if (isRecording && !gesture.selected)
            EndRecording();
#endif
        //if speaking for longer than MAX_DURATION, stop the recording
        if (isRecording)
        {
            time += Time.deltaTime;
            if (time >= MAX_DURATION)
            {
                StopRecording2();
            }
        }
    }
    private async void CreateJSON(string request) 
    {
        Debug.Log("inside CreateJSON: " + inputBox.text);

        ChatMessage userRequest = new ChatMessage() {
            Role = "user",
            Content = request
        };

        messages.Add(userRequest);

        outputBox.text = "Loading response...";

        // Complete the instruction
        try 
        {
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest() {
                Model = "gpt-3.5-turbo-16k",
                Messages = messages,
                Temperature = 0f,
                MaxTokens = 256,
                PresencePenalty = 0,
                FrequencyPenalty = 0
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0) 
            {
                var aiResponse = completionResponse.Choices[0].Message;
                aiResponse.Content = aiResponse.Content.Trim();

                messages.Add(aiResponse);
                outputBox.text = aiResponse.Content; //displaying LLM response to UI

                Debug.Log("CommandInterpreter message: " + aiResponse.Content); //[[[CONSIDER MOVING THIS TO DEBUG SUITE]]]
                WorldStateManager.Instance.BuildCommandBatch(aiResponse.Content, userRequest.Content);
            } 
            else 
            {
                outputBox.text = "No text was generated from this prompt.";
            }
        } 
        catch (System.Exception e) {
            outputBox.text = e.Message;
        }
    }
}