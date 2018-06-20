using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeSelector : NodeBase
	{
		public override IEnumerator RunningRoutine()
		{
			nodeState = NodeState.Running;
			
			var enumrator = childNodes.GetEnumerator();
			while(enumrator.MoveNext())
			{
				yield return baseTree.StartCoroutine(enumrator.Current.RunningRoutine());
				if (enumrator.Current.nodeState == NodeState.Success)
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
