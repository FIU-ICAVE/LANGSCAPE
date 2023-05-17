using OpenAI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private KeyCode PUSH_TO_TALK_KEY = KeyCode.V;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;

    private readonly string fileName = "output.wav";
    private readonly int MAX_DURATION = 30;

    private AudioClip clip;
    private bool isRecording;
    private float time;
    private OpenAIApi openai = new OpenAIApi(apiKey: "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");

    private void Start() {
        dropdown.ClearOptions();
        foreach (var device in Microphone.devices) {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
    }

    private void ChangeMicrophone(int index) {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording() {
        isRecording = true;
        symbol.enabled = true;

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        clip = Microphone.Start(dropdown.options[index].text, false, MAX_DURATION, 44100);
    }

    private async void EndRecording() {
        isRecording = false;
        symbol.enabled = false;

        message.text = "Transcripting...";

        Microphone.End(null);
        byte[] data = SaveWav.Save(fileName, clip);

        var req = new CreateAudioTranscriptionsRequest {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            // File = Application.persistentDataPath + "/" + fileName,
            Model = "whisper-1",
            Language = "en"
        };
        var res = await openai.CreateAudioTranscription(req);

        message.text = res.Text;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRecording && Input.GetKeyDown(PUSH_TO_TALK_KEY))
            StartRecording();
        if (isRecording && Input.GetKeyUp(PUSH_TO_TALK_KEY))
            EndRecording();
    }
}
