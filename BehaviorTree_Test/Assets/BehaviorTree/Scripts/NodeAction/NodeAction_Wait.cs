using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeAction_Wait : NodeActionBase
    {
        [NodeAttribute("Time", NodeAttributeOptionType.Required)] float m_Time = -1;
        [NodeAttribute("IsRealtime", NodeAttributeOptionType.Optional)] bool m_IsRealtime = false;


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