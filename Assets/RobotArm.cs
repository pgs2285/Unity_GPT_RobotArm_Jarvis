using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RobotArm : MonoBehaviour
{
    private bool isAttachOn = false;
    public Vector3 offset = new Vector3(0, 0, 0);
    public GameObject grabObject = null;
    private Vector3 relativePosition;
    private Quaternion relativeRotation;

    public void AttackMode(bool mode)
    {
        isAttachOn = mode;
    }

    private void OnTriggerEnter(Collider collision)
    {
        grabObject = collision.gameObject;
        //grabObect.transform.SetParent(gameObject.transform, true);
        grabObject.GetComponent<Rigidbody>().isKinematic = true;

        // 상대적 위치 저장
        relativePosition = grabObject.transform.position - transform.position;

    }

    private void Update()
    {

        if (isAttachOn && grabObject != null)
        {
            // 상대적 위치를 유지하면서 위치 업데이트
            grabObject.transform.position = transform.up + transform.position;
        }
    }
}
