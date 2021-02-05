using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeAction_Wait : NodeActionBase
    {
        float m_Time = -1;
        bool m_IsRealtime = false;

        public override void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
        {
            base.Setup(xmlAttributes, baseTree);

            m_Time = -1;
            if (xmlAttributes["Time"] != null &&
                string.IsNullOrEmpty(xmlAttributes["Time"].Value) == false)
            {
                if (float.TryParse(xmlAttributes["Time"].Value, out m_Time) == false)
                    m_Time = -1;
            }

            m_IsRealtime = false;
            if (xmlAttributes["IsRealtime"] != null &&
                string.IsNullOrEmpty(xmlAttributes["IsRealtime"].Value) == false)
            {
                if (bool.TryParse(xmlAttributes["IsRealtime"].Value, out m_IsRealtime) == false)
                    m_IsRealtime = false;
            }
        }

        public override IEnumerator RunningRoutine()
        {
            nodeState = NodeState.Running;

            if (m_Time > 0)
            {
                if (m_IsRealtime == true)
                {
                    yield return new WaitForSecondsRealtime(m_Time);
                }
                else
                {
                    yield return new WaitForSeconds(m_Time);
                }
            }

            nodeState = NodeState.Success;
        }
    }
}