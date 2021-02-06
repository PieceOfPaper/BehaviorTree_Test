using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleRobotKyle : MonoBehaviour
{


    float m_AliveTime = 0f;
    public float AliveTime => m_AliveTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_AliveTime += Time.deltaTime;
    }
}
