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
			State = CheckCondition() ? NodeState.Success : NodeState.Fail;

			if (State == NodeState.Success)
			{
				NodeBase[] nodes = GetAllChildren();
				for (int i = 0; i < nodes.Length; i++)
				{
					yield return baseTree.StartCoroutine(nodes[IsReverse ? nodes.Length - i - 1 : i].RunningRoutine());
				}
			}
		}
	}
}
