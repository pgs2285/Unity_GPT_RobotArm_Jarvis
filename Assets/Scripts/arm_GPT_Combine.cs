using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;

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
        _mode = Mode.none; // ?????????? none
    }

    private void Update()
    {
        if (_mode == Mode.pick && _hand.grabObject != null)
        {
            returnHome();
            _mode = Mode.none;
        }
        else if (_mode == Mode.drop)
        {
            _hand.GetComponent<Collider>().enabled = false;
            _hand.grabObject = null;
        }
        else if (_mode == Mode.move && _hand.grabObject == null)
        {
            returnHome();
            _mode = Mode.none;
        }

    }
    #endregion

    #region Other Methods

    private void returnHome()
    {
        ikManager.m_target = GameObject.FindGameObjectWithTag("Home");
    }
    #region TMP_ver
    public async void MoveArmBasedOnCommand(TextMeshProUGUI textMeshPro)
    {
        string command = textMeshPro.text;
        string response = await chatGptController.SendMessageToChatGPT(command);
        
        Debug.Log(response);
        if (response != null)
        {
            if (response.Contains("move to"))
            {
                string[] parts = response.Split(new string[] { " move to " }, System.StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    string objectNameA = parts[0].Trim();
                    string objectNameB = parts[1].Trim();
                    GameObject objectA = GameObject.Find(objectNameA);
                    GameObject objectB = GameObject.Find(objectNameB);
                    if (objectA != null && objectB != null)
                    {
                        // Step 1: Move to objectA to pick it up
                        _mode = Mode.pick;
                        ikManager.m_target = objectA;
                        _hand.AttackMode(true);

                        // Wait until the object is picked up
                        StartCoroutine(WaitForPickup(objectB));
                    }
                    else
                    {
                        if (objectA == null) Debug.LogWarning("Object not found: " + objectNameA);
                        if (objectB == null) Debug.LogWarning("Object not found: " + objectNameB);
                    }
                }
                else
                {
                    Debug.LogWarning("Invalid move to command format: " + response);
                }
            }
            else if (response.StartsWith("pick up"))
            {
                _hand.GetComponent<Collider>().enabled = true; // ?????? ??????
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
                //DisplayChatResponse(response); // else???? ???? ???? ?????? ????
            }
        }
    }
    #endregion
    #region string_ver
    public async Task<string> MoveArmBasedOnCommand(string text)
    {
        string command = text;
        string response = await chatGptController.SendMessageToChatGPT(command);

        Debug.Log(response);
        if (response != null)
        {
            if (response.Contains("move to"))
            {
                _hand.GetComponent<Collider>().enabled = true; // ?????? ??????
                string[] parts = response.Split(new string[] { " move to " }, System.StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    string objectNameA = parts[0].Trim();
                    string objectNameB = parts[1].Trim();
                    GameObject objectA = GameObject.Find(objectNameA);
                    GameObject objectB = GameObject.Find(objectNameB);
                    if (objectA != null && objectB != null)
                    {
                        // Step 1: Move to objectA to pick it up
                        _mode = Mode.pick;
                        ikManager.m_target = objectA;
                        _hand.AttackMode(true);

                        // Wait until the object is picked up
                        StartCoroutine(WaitForPickup(objectB));
                    }
                    else
                    {
                        if (objectA == null) Debug.LogWarning("Object not found: " + objectNameA);
                        if (objectB == null) Debug.LogWarning("Object not found: " + objectNameB);
                    }
                }
                else
                {
                    Debug.LogWarning("Invalid move to command format: " + response);
                }
                return "of course :)";
            }
            else if (response.StartsWith("pick up"))
            {
                _hand.GetComponent<Collider>().enabled = true; // ?????? ??????
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
                return "of course :)";
            }
            else if (response.StartsWith("drop"))
            {
                _mode = Mode.drop;
                string objectName = response.Substring("drop ".Length).Trim();
                GameObject targetObject = GameObject.Find(objectName);
                _hand.grabObject.GetComponent<Rigidbody>().isKinematic = false;
                _hand.AttackMode(false);
                return "of course :)";
            }

        }
        return response;
    }
    #endregion
    private IEnumerator WaitForPickup(GameObject targetObject)
    {
        // ?????? ???? ????????
        while (_mode == Mode.pick && _hand.grabObject == null)
        {
            yield return null;
        }

        _mode = Mode.move;
        ikManager.m_target = targetObject;

        // ?????? ?????? ?????? ????
        while (_mode == Mode.move && Vector3.Distance(_hand.transform.position, targetObject.transform.position) > 2f)
        {
            yield return null;
        }

        // ?????? ???????? ?????? ???????? grabObject?? null?? ????
        if (_mode == Mode.move)
        {
            _hand.AttackMode(false);
            _hand.grabObject = null;
            returnHome();
            _mode = Mode.none;
        }
    }
    #endregion
}
