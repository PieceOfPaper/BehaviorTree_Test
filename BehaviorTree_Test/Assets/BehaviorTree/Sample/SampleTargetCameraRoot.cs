using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTargetCameraRoot : MonoBehaviour
{
    public Transform m_Target;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Target != null)
            transform.position = m_Target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target != null && m_Target.hasChanged)
            transform.position = m_Target.position;
    }
}
