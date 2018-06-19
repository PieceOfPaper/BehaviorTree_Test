using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class BehaviorTree : MonoBehaviour
	{
		protected NodeBase _rootNode = null;
		public NodeBase rootNode
		{
			protected set
			{
				_rootNode = value;
			}
			get
			{
				return _rootNode;
			}
		}

		protected NodeBase _currentNode = null;
		public NodeBase currentNode
		{
			protected set
			{
				_currentNode = value;
			}
			get
			{
				return _currentNode;
			}
		}

		public void Run()
		{
		}

		IEnumerator RunRoutine()
		{
			yield return null;
		}
	}
}
