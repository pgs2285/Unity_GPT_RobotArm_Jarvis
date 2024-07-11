using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    // Root of the armature
    public Joint m_root;
    //End Effector
    public Joint m_end;
    public GameObject m_target;
    public float m_threshold = 0.05f;

    public float m_rate = 10.0f;

    public int m_steps = 20;

    float CalculateSlope(Joint _joint, Vector3 axis)
    {
        float deltaTheta = 0.01f;
        float distance1 = GetDistance(m_end.transform.position, m_target.transform.position);
        _joint.Rotate(axis * deltaTheta);

        float distance2 = GetDistance(m_end.transform.position, m_target.transform.position);
        _joint.Rotate(axis * -deltaTheta);

        return (distance2 - distance1) / deltaTheta;
    }

    private void Update()
    {
        if (m_target == null) return;
        for (int i = 0; i < m_steps; ++i)
        {
            if (GetDistance(m_end.transform.position, m_target.transform.position) > m_threshold)
            {
                Joint current = m_root;
                while (current != null)
                {
                    // 각 축에 대해 회전각도를 계산하고 적용합니다.
                    Vector3[] axes = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
                    foreach (Vector3 axis in axes)
                    {
                        float slope = CalculateSlope(current, axis);
                        current.Rotate(axis * -slope * m_rate);
                    }
                    current = current.GetChild();
                }
            }
        }
    }

    float GetDistance(Vector3 _point, Vector3 _point2)
    {
        return Vector3.Distance(_point, _point2);
    }
}
