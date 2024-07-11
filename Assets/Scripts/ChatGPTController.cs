using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChatGPTController : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    #region API_KEY
    private string apiKey = "sk-proj-eSHfyLCXxBq9aVvi53PwT3BlbkFJVwtX0xgm2etwFTShrhTt";
    #endregion
    private string apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

    public async Task<string> SendMessageToChatGPT(string message)
    {
        var requestBody = new
        {
            prompt = $"You are controlling a robotic arm. The arm can perform the following actions: 'move [object_name] to [object_name]', 'pick up [object_name]', 'drop [object_name]'. Respond with the exact command based on the user's request.\nUser: {message}\nResponse:",
            max_tokens = 50,
            temperature = 0.7
        };

        var requestJson = JsonUtility.ToJson(requestBody);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await client.PostAsync(apiUrl, content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var responseObject = JsonUtility.FromJson<ChatGPTResponse>(responseJson);
        return responseObject.choices[0].text.Trim();
    }
}

[Serializable]
public class ChatGPTResponse
{
    public Choice[] choices;
}

[Serializable]
public class Choice
{
    public string text;
}
