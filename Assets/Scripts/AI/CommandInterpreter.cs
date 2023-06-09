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
        messages.Clear();
        // TODO: Modify the prompt to include data depending on command history and player position
        string prompt = "Create an empty 20x20x20 grid. Replace vectors like so: \"x y z\", where positions and sizes are vectors. Replace block a value from Empty: 0, Solid: 1, Glass: 2, Grid: 3, Filter: 4. Replace color using hexadecimal and ignore opacity. None of the parameters are optional. The x, y, and z axis represent right and width, up and height, and forward and length respectively. Do NOT respond with anything but commands to achieve my requests, and separate commands with new lines.\r\n\r\nCommands:\r\nUse \"n\" if the instructions are not valid, and do not explain anything.\r\nUse \"f <position> <size> <block> [color]\" to fill an area starting from position, with the dimensions of size using block of color.\r\n\r\nRules:\r\nWalls lie either on the XY plane or YZ plane and the last axis equals 1. Floors and ceilings lie on the XZ plane, and the Y axis equals 1. Windows are walls made of glass.\r\n\r\nExamples:\r\nSet a grid block at 0 0 0: f 0 0 0 1 1 1 3\r\nBuild a wall along the XY plane: f 0 0 0 10 10 1 1\r\nBuild a wall along the YZ plane: f 0 0 0 1 10 10 1\r\nBuild a blue wall that is 10 blocks tall and 3 blocks wide at the origin: f 0 0 0 3 10 1 1 #0000ff\r\nBuild a wall that is 10x5 at 0 0 0: f 0 0 0 10 5 1 1\r\nCreate a red floor: f 0 0 0 10 1 10 1 #ff0000.\r\nCreate a 10x5 window at the origin: f 0 0 0 10 5 1 2";
        messages.Add(new ChatMessage() { Role = "system", Content = prompt });

        ChatMessage userInstructions = new ChatMessage() {
            Role = "user",
            Content = command
        };

        messages.Add(userInstructions);

        outputBox.text = "Loading response...";

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

#if TEST_CUBEPLACER
            WorldCommand commands = JSONParser.ParseCommand(message.Content);
            if (commands != null && commands.success && commands.modified != null) {
                Debug.Log("Placing: " + commands.modified.Length);
                GridMesh.Instance.Multiplace(commands.modified);
            }
#else
            Debug.Log("CommandInterpreter message: " + message.Content.ToString()); //[[[CONSIDER MOVING THIS TO DEBUG SUITE]]]
            WorldStateManager.Instance.BuildCommand(message.Content.ToString());
#endif
        } else {
#if UNITY_EDITOR
            Debug.LogWarning("No text was generated from this prompt.");
#endif
            outputBox.text = "No text was generated from this prompt.";
        }
    }
}