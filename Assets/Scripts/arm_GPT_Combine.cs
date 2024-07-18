using UnityEngine;
using TMPro;

public class arm_GPT_Combine : MonoBehaviour
{
    enum Mode
    {
        move,pick,drop,none
    }

    #region Variables
    public Transform target;
    private OpenAI.ChatGPTController chatGptController;
    private IKManager ikManager;
    private RobotArm _hand;
    private Mode _mode;
    #endregion

    #region Unity Methods
    void Awake()
    {
        chatGptController = gameObject.GetComponent<OpenAI.ChatGPTController>();
        ikManager = gameObject.GetComponent<IKManager>();
        _hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<RobotArm>();
        _mode = Mode.none; // 초기모드는 none
    }

    private void Update()
    {
        if(_mode == Mode.pick && _hand.grabObject != null)
        {
            returnHome(); 
            _mode = Mode.none;
        }
        else if(_mode == Mode.drop)
        {
            _hand.GetComponent<Collider>().enabled = false;
            _hand.grabObject = null;
        }
    }
    #endregion

    #region Other Methods

    private void returnHome()
    {
        ikManager.m_target = GameObject.FindGameObjectWithTag("Home");
    }
    public async void MoveArmBasedOnCommand(TextMeshProUGUI textMeshPro)
    {
        string command = textMeshPro.text;
        string response = await chatGptController.SendMessageToChatGPT(command);

        Debug.Log(response);
        if (response != null)
        {
            if (response.StartsWith("move to"))
            {
                string objectName = response.Substring("move to ".Length).Trim();
                GameObject targetObject = GameObject.Find(objectName);
                _mode = Mode.move;
                if (targetObject != null)
                {
                    target.position = targetObject.transform.position;
                    target.rotation = targetObject.transform.rotation;
                    _hand.GetComponent<RobotArm>().AttackMode(true);
                }
                else
                {
                    Debug.LogWarning("Target object not found: " + objectName);
                }
            }
            else if (response.StartsWith("pick up"))
            {
                _hand.GetComponent<Collider>().enabled = true;
                string objectName = response.Substring("pick up ".Length).Trim();
                Debug.Log(objectName);
                GameObject targetObject = GameObject.Find(objectName);
                _mode = Mode.pick;

                if (targetObject != null)
                {
                    ikManager.m_target = targetObject;
                    _hand.GetComponent<RobotArm>().AttackMode(true);
                }
                else
                {
                    Debug.LogWarning("Target object not found: " + objectName);
                }
            }
            else if (response.StartsWith("drop"))
            {
                _mode = Mode.drop;
                string objectName = response.Substring("drop ".Length).Trim();
                GameObject targetObject = GameObject.Find(objectName);
                _hand.grabObject.GetComponent<Rigidbody>().isKinematic = false;
                _hand.AttackMode(false);
            }
            else
            {
                Debug.LogWarning("Unknown command received from ChatGPT: " + response);
            }
        }
    }
    #endregion
}
