using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeSequence : NodeBase
	{
		public override IEnumerator RunningRoutine()
		{
			NodeBase[] nodes = GetAllChildren();
			nodeState = NodeState.Running;
			
			for (int i = 0; i < nodes.Length; i ++)
			{
				yield return baseTree.StartCoroutine( nodes[IsReverse ? nodes.Length - i - 1 : i ].RunningRoutine() );
				if (nodes[IsReverse ? nodes.Length - i - 1 : i ].nodeState == NodeState.Fail)
				{
					nodeState = NodeState.Fail;
					break;
				}
			}
			
			if (nodeState != NodeState.Fail)
				nodeState = NodeState.Success;
				
			ResetChildrenState();
		}
	}
}
