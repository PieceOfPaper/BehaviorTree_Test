using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeSequence : NodeBase
	{
		public override IEnumerator RunningRoutine()
		{
			nodeState = NodeState.Running;
			
			var enumrator = childNodes.GetEnumerator();
			while(enumrator.MoveNext())
			{
				yield return baseTree.StartCoroutine(enumrator.Current.RunningRoutine());
				if (enumrator.Current.nodeState == NodeState.Fail)
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
