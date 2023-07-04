/*
 * Author: Jose Gonzalez Lopez, Christian Laverde, Justin Cardoso
 * Script Description:
 *      Handles converting voice input into valid JSON object.
 * NOTES:
 *      - consider moving Debug.Log() that outputs the command to DebugSuite
 * TO DO:
 *      - Interpret JSON string to valid JSON
 *      - Expand on the instructions for the prompt
 *      - Enable speech recognition via OVR
 */

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
    // Display
    [SerializeField] private TMP_Text inputBox;
    [SerializeField] private TMP_Text outputBox;
    [SerializeField] private UnityEngine.UI.Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;

    // OpenAI Settings
    private OpenAIApi openai;
    private string prompt;

    // Whisper
    private readonly string fileName = "output.wav";
    private readonly int MAX_DURATION = 30;
    private AudioClip clip;
    private bool isRecording;
    private float time = 0;

    //Gesture Detect
#if !UNITY_STANDALONE_WIN
    public GestureTest gesture;
#endif

    // ChatGPT
    private List<ChatMessage> messages = new List<ChatMessage>();

    // Loads prompt from file in Assets/Resources/prompt
    void Awake() {
        openai = new OpenAIApi(apiKey: "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");
        TextAsset filedata = Resources.Load<TextAsset>("OpenAI/PROMPT");
        if (filedata == null)
            throw new System.Exception("No file found called prompt in 'Assets/Resources/OpenAI/PROMPT");
        prompt = filedata.text;
        Debug.Log(prompt);
    }

    private void Start() {
        dropdown.ClearOptions();
        foreach (var device in Microphone.devices) {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        dropdown.onValueChanged.AddListener(ChangeMicrophone);
        inputBox.text = "...";
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();

        messages.Add(new ChatMessage() { Role = "system", Content = prompt });
    }
    private void ChangeMicrophone(int index) {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }
    private void StartRecording() {
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
    }

    // Update is called once per frame
    void Update() {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.V))
            StartRecording();
        if (Input.GetKeyUp(KeyCode.V))
            EndRecording();
#else
        if (!isRecording && gesture.selected)
            StartRecording();
        if (isRecording && !gesture.selected)
            EndRecording();
#endif

        if (isRecording) {
            time += Time.deltaTime;
            if (time >= MAX_DURATION)
                EndRecording();
        }
    }
    private async void CreateJSON(string request) {
        ChatMessage userRequest = new ChatMessage() {
            Role = "user",
            Content = request
        };

        messages.Add(userRequest);

        outputBox.text = "Loading response...";

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
                outputBox.text = aiResponse.Content;

#if TEST_CUBEPLACER
            WorldCommand commands = JSONParser.ParseCommand(message.Content);
            if (commands != null && commands.success && commands.modified != null) {
                Debug.Log("Placing: " + commands.modified.Length);
                GridMesh.Instance.Multiplace(commands.modified);
            }
#else
                //Debug.Log("CommandInterpreter message: " + message.Content.ToString()); //[[[CONSIDER MOVING THIS TO DEBUG SUITE]]]
                WorldStateManager.Instance.BuildCommandBatch(aiResponse.Content, userRequest.Content);
#endif
            } else {
                outputBox.text = "No text was generated from this prompt.";
            }
        } catch (System.Exception e) {
            outputBox.text = e.Message;
        }
    }
}