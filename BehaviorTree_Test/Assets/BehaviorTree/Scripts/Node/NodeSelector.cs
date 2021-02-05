using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeSelector : NodeBase
	{
		protected override void SetNodeByXmlAttributes(XmlAttributeCollection xmlAttributes)
		{
			// nothing
		}

		public override IEnumerator RunningRoutine()
		{
			NodeBase[] nodes = GetAllChildren();
			nodeState = NodeState.Running;
			
			for (int i = 0; i < nodes.Length; i ++)
			{
				yield return baseTree.StartCoroutine( nodes[ isReverse ? nodes.Length - i - 1 : i ].RunningRoutine() );
				if (nodes[ isReverse ? nodes.Length - i - 1 : i ].nodeState == NodeState.Success)
				{
					nodeState = NodeState.Success;
					break;
				}
			}
			
			if (nodeState != NodeState.Success)
				nodeState = NodeState.Fail;

			ResetChildrenState();
		}
    }
}
