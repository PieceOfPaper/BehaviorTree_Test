using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeSelector : NodeBase
	{
		public override IEnumerator RunningRoutine()
		{
			NodeBase[] nodes = GetAllChildren();
			State = NodeState.Running;
			
			for (int i = 0; i < nodes.Length; i ++)
			{
				yield return baseTree.StartCoroutine( nodes[IsReverse ? nodes.Length - i - 1 : i ].RunningRoutine() );
				if (nodes[IsReverse ? nodes.Length - i - 1 : i ].State == NodeState.Success)
				{
					State = NodeState.Success;
					break;
				}
			}
			
			if (State != NodeState.Success)
				State = NodeState.Fail;

			ResetChildrenState();
		}
    }
}
