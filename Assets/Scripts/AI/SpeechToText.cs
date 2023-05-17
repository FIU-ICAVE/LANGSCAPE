using OpenAI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private KeyCode PUSH_TO_TALK_KEY = KeyCode.V;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;

    private readonly string fileName = "output.wav";
    private readonly int MAX_DURATION = 20;

    private AudioClip clip;
    private bool isRecording;
    private float time = 0;
    private OpenAIApi openai = new OpenAIApi(apiKey: "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");

    [SerializeField] private string startKeyword;
    [SerializeField] private string stopKeyword;
    private KeywordRecognizer keywordRecognizer;

    private void Start() {
        dropdown.ClearOptions();
        foreach (var device in Microphone.devices) {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        
        dropdown.onValueChanged.AddListener(ChangeMicrophone);
        message.text = "...";

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();

        string[] keywords = { startKeyword, stopKeyword };

        keywordRecognizer = new KeywordRecognizer(keywords, ConfidenceLevel.Medium);
        keywordRecognizer.OnPhraseRecognized += OnCommandRecognition;
        keywordRecognizer.Start();
    }

    private void OnCommandRecognition(PhraseRecognizedEventArgs args) {
        if (!isRecording && args.text == startKeyword)
            StartRecording();
        else if (isRecording && args.text == stopKeyword)
            EndRecording();
    }

    private void ChangeMicrophone(int index) {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording() {
        isRecording = true;
        symbol.enabled = true;
        message.text = "Listening...";

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        clip = Microphone.Start(dropdown.options[index].text, false, MAX_DURATION, 44100);
    }

    private async void EndRecording() {
        isRecording = false;
        symbol.enabled = false;
        time = 0;

        message.text = "Transcribing...";

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

        if (isRecording) {
            time += Time.deltaTime;
            if (time >= MAX_DURATION)
                EndRecording();
        }
    }

    private void OnApplicationQuit() {
        keywordRecognizer.Stop();
        keywordRecognizer.Dispose();
    }
}
