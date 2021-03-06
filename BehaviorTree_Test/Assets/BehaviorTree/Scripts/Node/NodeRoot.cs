using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeRoot : NodeBase
    {
        public override IEnumerator RunningRoutine()
		{
			NodeBase[] nodes = GetAllChildren();
			State = NodeState.Running;

			for (int i = 0; i < nodes.Length; i++)
				yield return baseTree.StartCoroutine(nodes[IsReverse ? nodes.Length - i - 1 : i].RunningRoutine());

			State = NodeState.Success;
			ResetChildrenState();
		}
    }
}
