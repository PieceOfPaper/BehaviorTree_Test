using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


namespace BehaviorTree
{
    public class NodeAction_AnimatorState : NodeActionBase
    {
        [NodeAttribute("StateName", NodeAttributeOptionType.Required)] string m_StateName;
        [NodeAttribute("Time", NodeAttributeOptionType.Optional)] float m_Time = -1;

        Animator m_Animator;
        int m_StateHash;


        public override void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
        {
            base.Setup(xmlAttributes, baseTree);

            m_Animator = baseTree == null ? null : baseTree.GetComponentInChildren<Animator>();
            m_StateHash = Animator.StringToHash(m_StateName);
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
