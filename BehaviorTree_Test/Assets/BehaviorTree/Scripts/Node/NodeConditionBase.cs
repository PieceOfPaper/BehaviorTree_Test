using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{ 
	public abstract class NodeConditionBase : NodeBase
	{
		public abstract bool CheckCondition();



		public override IEnumerator RunningRoutine()
		{
			nodeState = CheckCondition() ? NodeState.Success : NodeState.Fail;

			if (nodeState == NodeState.Success)
			{
				NodeBase[] nodes = GetAllChildren();
				for (int i = 0; i < nodes.Length; i++)
				{
					yield return baseTree.StartCoroutine(nodes[isReverse ? nodes.Length - i - 1 : i].RunningRoutine());
				}
			}
		}
	}
}