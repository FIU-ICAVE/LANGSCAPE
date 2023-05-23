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
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;
//using UnityEngine.Windows.Speech;

public class CommandInterpreter : MonoBehaviour
{
    // Display
    [SerializeField] private CubePlacer cubePlacer;
    [SerializeField] private KeyCode PUSH_TO_TALK_KEY = KeyCode.V;
    [SerializeField] private TMP_Text inputBox;
    [SerializeField] private TMP_Text outputBox;
    [SerializeField] private Image symbol;
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
    public GestureTest gesture;

    // ChatGPT
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = 
        "Only respond in JSON and if you cannot follow the sample format, return success as 0. Do not respond with any text " +
        "Let's play a game " +
        "create a 50 by 50 by 50 3D grid " +
        "you can create a block of any color with any opacity at any position in this grid, you will place a block when I say so " +
        "return the properties of the block in this JSON format {\"data\":[{\"success\":true,\"type\":\"block\",\"color\": {\"r\":1.0,\"g\":1.0,\"b\":1.0, \"a\":1.0},\"position\":{\"x\":0,\"y\":0,\"z\":0}}]}.";

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

        
        //var index = PlayerPrefs.GetInt("user-mic-device-index");
        clip = Microphone.Start(dropdown.options[0].text, false, MAX_DURATION, 44100);
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
        if (res.Text != "")
            CreateJSON(res.Text);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRecording && gesture.selected)
            StartRecording();
        if (isRecording && !gesture.selected)
            EndRecording();

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

        if (messages.Count == 0) newMessage.Content = prompt + "\n" + command;

        messages.Add(newMessage);
        outputBox.text = "Loading...";

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest() {
            Model = "gpt-3.5-turbo-0301",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0) {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();

            messages.Add(message);
            outputBox.text = message.Content;

            JSONWrapper<ObjectData> obj = JSONParser.FromJSON(message.Content);
            if (obj != null)
            {
                cubePlacer.PlaceCube(obj.data[0]);
                Debug.Log(obj.data[0].color);
            }
        } else {
            Debug.LogWarning("No text was generated from this prompt.");
        }
    }
}
