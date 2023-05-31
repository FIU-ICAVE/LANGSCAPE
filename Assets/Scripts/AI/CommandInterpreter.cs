/*
 * Author: Jose Gonzalez Lopez, Christian Laverde, Justin Cardoso
 * Script Description:
 *      Handles converting voice input into valid JSON object.
 *      
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
#if !UNITY_STANDALONE_WIN
using Oculus.Interaction;
//using UnityEngine.Windows.Speech;
#endif

public class CommandInterpreter : MonoBehaviour {
    // Display
    [SerializeField] private CubePlacer cubePlacer;
    [SerializeField] private TMP_Text inputBox;
    [SerializeField] private TMP_Text outputBox;
    [SerializeField] private UnityEngine.UI.Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;

    // OpenAI Settings
    private OpenAIApi openai = new OpenAIApi(apiKey: "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");

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
    private string prompt =
        "Only respond in JSON and if you cannot follow the sample format, return success as false with an empty modified array. " +
        "Do not respond with any text. " +
        "Let's play a game, create an empty 30 by 30 by 30 3D grid where each cell corresponds to an int. " +
        "0 corresponds to empty, 1 corresponds to a solid block, 2 corresponds to glass, and 3 corresponds to a chair. " +
        "You can create any of these at any position in this grid, you will place blocks when I say so. " +
        "For each block modified by my commands, return the ID of the object to place, as well as the original_position of the object on the grid and the new_position it was moved to. " +
        "If a new item is placed on the grid, return original_position as (-1,-1,-1), and if no color is specified then always default to white with 0.6 alpha. " +
        "Follow this format: " +
        "{\"success\":true,\"modified\":[{\"type\":1,\"original_position\":{\"x\":-1,\"y\":-1,\"z\":-1},\"new_position\":{\"x\":-1,\"y\":-1,\"z\":-1},\"color\":{\"r\":1.0,\"g\":1.0,\"b\":1.0,\"a\":1.0}}]}. " +
        "The world vectors are up: (0,1,0), right:(1,0,0), and forward: (0,0,1).";

    //"{\"success\":true,\"modified\":[{\"type\":0,\"original_position\":{\"x\":-1,\"y\":-1,\"z\":-1},\"new_position\":{\"x\":-1,\"y\":-1,\"z\":-1},\"color\":{\"r\":1.0,\"g\":1.0,\"b\":1.0,\"a\":1.0}}]}. "

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
    private async void CreateJSON(string command) {
        var newMessage = new ChatMessage() {
            Role = "user",
            Content = command
        };

        if (messages.Count == 0) 
            newMessage.Content = prompt + "\n" + command;

        messages.Add(newMessage);
        outputBox.text = "Loading...";

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest() {
            Model = "gpt-3.5-turbo",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0) {
            var message = completionResponse.Choices[0].Message;
            for (int i = 1; i < completionResponse.Choices.Count; i++)
                message.Content += completionResponse.Choices[i].Message.Content;
            message.Content = message.Content.Trim();

            //messages.Add(message);
            outputBox.text = message.Content;

            WorldCommand commands = JSONParser.ParseCommand(message.Content);
            if (commands != null && commands.success && commands.modified != null) {
                Debug.Log("Placing: " + commands.modified.Length);
                GridMesh.Instance.Multiplace(commands.modified);
            }
        } else {
            Debug.LogWarning("No text was generated from this prompt.");
            outputBox.text = "You done goofed up.";
        }
    }
}