using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI
{
    public class ChatGPTController : MonoBehaviour
    {
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are controlling a robotic arm. The arm can perform the following actions: '[object_name] move to [object_name]', 'pick up [object_name]', 'drop [object_name]'. insert object name. Respond with the exact command based on the user's request. \nUser: {0}\nResponse:";
        
        public async Task<string> SendMessageToChatGPT(string userInput)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = userInput
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + userInput; 

            messages.Add(newMessage);

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                return message.Content;
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
                return null;
            }
        }
    }
}