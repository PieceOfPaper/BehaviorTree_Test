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

		protected bool _isRunning = false;
		public bool isRunning
		{
			protected set
			{
				_isRunning = value;
			}
			get
			{
				return _isRunning;
			}
		}

		Coroutine runRoutine;

		public void Run()
		{
			if (isRunning) return;
			if (runRoutine != null) StopCoroutine(runRoutine);
			runRoutine = StartCoroutine(RunRoutine());
		}

		public void Stop()
		{
			isRunning = false;
			if (runRoutine != null) StopCoroutine(runRoutine);
		}

		protected IEnumerator RunRoutine()
		{
			isRunning = true;
			while(rootNode != null)
			{
				yield return StartCoroutine(rootNode.RunningRoutine());
			}
			isRunning = false;
		}
	}
}
