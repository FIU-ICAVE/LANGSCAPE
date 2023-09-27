using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Search;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi(apiKey: "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");
        
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";
        
        private SearchAlgorithms sa = new SearchAlgorithms();

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            // Command Indicators for Only Instructions 
            // :: 1 for Only Commands, 2 for Words and Commands, 3 for 2nd LLM Keyword ::
            string[] indicator = { "f ", "m ", "r ", "c ", "u ", "v ", "q ", "t " };
            string[] indicator2 = { " f ", " m ", " r ", " c ", " u ", " v ", " q ", " t " };
            string[] LLM_keyword = { "Background", "background" };

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };

            // If User Input has key indicator "background" or "Background", Switch to Second LLM
            if (sa.SwitchLLM(newMessage.Content, LLM_keyword[0]) || sa.SwitchLLM(newMessage.Content, LLM_keyword[1])){

                /*
                AppendMessage(newMessage);

                if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

                messages.Add(newMessage);
                */

                button.enabled = false;
                inputField.text = "";
                inputField.enabled = false;

                // Code for Second LLM ???

                button.enabled = true;
                inputField.enabled = true;
            }
            else
            {
                AppendMessage(newMessage);

                if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

                messages.Add(newMessage);

                button.enabled = false;
                inputField.text = "";
                inputField.enabled = false;

                // Complete the instruction
                var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                {
                    Model = "gpt-3.5-turbo-0301",
                    Messages = messages
                });

                if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
                {
                    var message = completionResponse.Choices[0].Message;

                    string fluff = string.Empty; // Sentence
                    string instruct = string.Empty; // Command

                    // If Message Contains Only the Command don't Modify, otherwise Modify
                    if (sa.hasOnlyCommand(message.Content, indicator) == true || message.Content == "n") {
                        var Updated = sa.commandOnly((string)message.Content, indicator2);

                        // Only The Instructions
                        instruct = Updated.command;
                        // Only the Message
                        fluff = Updated.sentence;
                        
                    }
                    // If Instruct Has No Instructions, Change to Fluff (For Now)
                    if (string.IsNullOrEmpty(instruct) == false) {
                        message.Content = instruct;
                    }
                    else
                    {
                        message.Content = fluff;
                    }
         
                    message.Content = message.Content.Trim();

                    messages.Add(message);
                    AppendMessage(message);


                    
                }
                else
                {
                    Debug.LogWarning("No text was generated from this prompt.");
                }

                button.enabled = true;
                inputField.enabled = true;
            }
        }
    }
}
