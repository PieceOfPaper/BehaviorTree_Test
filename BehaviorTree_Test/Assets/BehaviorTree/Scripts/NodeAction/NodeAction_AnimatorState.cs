using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


namespace BehaviorTree
{
    public class NodeAction_AnimatorState : NodeActionBase
    {
        Animator m_Animator;
        int m_StateHash;
        float m_Time = -1;


        public override void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
        {
            base.Setup(xmlAttributes, baseTree);

            m_Animator = baseTree.GetComponentInChildren<Animator>();

            m_StateHash = 0;
            if (xmlAttributes["StateName"] != null &&
                string.IsNullOrEmpty(xmlAttributes["StateName"].Value) == false)
            {
                m_StateHash = Animator.StringToHash(xmlAttributes["StateName"].Value);
            }

            m_Time = -1;
            if (xmlAttributes["Time"] != null &&
                string.IsNullOrEmpty(xmlAttributes["Time"].Value) == false)
            {
                if (float.TryParse(xmlAttributes["Time"].Value, out m_Time) == false)
                    m_Time = -1;
            }

        }

        public override IEnumerator RunningRoutine()
        {
            nodeState = NodeState.Running;

            m_Animator.Play(m_StateHash);
            var stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

            if (m_Time >= 0)
            {
                yield return new WaitForSeconds(m_Time);
            }
            else
            {
                yield return new WaitForSeconds(stateInfo.length);
            }

            nodeState = NodeState.Success;
        }
    }
}
