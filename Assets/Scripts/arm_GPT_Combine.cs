using UnityEngine;
using TMPro;

public class arm_GPT_Combine : MonoBehaviour
{
    public Transform target;
    private ChatGPTController chatGptController;
    private IKManager ikManager;

    void Start()
    {
        chatGptController = gameObject.AddComponent<ChatGPTController>();
        ikManager = gameObject.AddComponent<IKManager>();
    }

    public async void MoveArmBasedOnCommand(TextMeshProUGUI textMeshPro)
    {
        string command = textMeshPro.text;
        string response = await chatGptController.SendMessageToChatGPT(command);

        if (response.StartsWith("move to"))
        {
            string objectName = response.Substring("move to ".Length).Trim();
            GameObject targetObject = GameObject.Find(objectName);

            if (targetObject != null)
            {
                target.position = targetObject.transform.position;
                target.rotation = targetObject.transform.rotation;
            }
            else
            {
                Debug.LogWarning("Target object not found: " + objectName);
            }
        }
        else if (response.StartsWith("pick up"))
        {
            string objectName = response.Substring("pick up ".Length).Trim();
            GameObject targetObject = GameObject.Find(objectName);

            if (targetObject != null)
            {
                ikManager.m_target = targetObject;
            }
            else
            {
                Debug.LogWarning("Target object not found: " + objectName);
            }
        }
        else if (response.StartsWith("drop"))
        {
            string objectName = response.Substring("drop ".Length).Trim();
            GameObject targetObject = GameObject.Find(objectName);

            if (targetObject != null)
            {
                // Logic to drop the object
            }
            else
            {
                Debug.LogWarning("Target object not found: " + objectName);
            }
        }
        else
        {
            Debug.LogWarning("Unknown command received from ChatGPT: " + response);
        }
    }
}
