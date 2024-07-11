using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
    public Joint m_child;

    public Joint GetChild()
    {
        return m_child;
    }

    public void Rotate(Vector3 eulerAngles)
    {
        transform.Rotate(eulerAngles);
    }
}
