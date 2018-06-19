using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public enum NodeState
	{
		None = 0,
		Success,
		Fail,
		Running,
	}

	public class NodeBase : MonoBehaviour 
	{
		protected NodeBase _parentNode;
		public virtual NodeBase parentNode
		{
			protected set
			{
				_parentNode = value;
			}
			get
			{
				return _parentNode;
			}
		}

		protected List<NodeBase> childNodes = new List<NodeBase>();

		protected NodeState _nodeState;
		public virtual NodeState nodeState
		{
			protected set
			{
				_nodeState = value;
			}
			get
			{
				return _nodeState;
			}
		}


		public virtual bool AddChildNode(NodeBase node, int index = -1)
		{
			if (childNodes == null) return false;
			//if (childNodes.Contains(node)) return false;

			if (index == -1) childNodes.Add(node);
			else childNodes.Insert(index, node);

			node.parentNode = this;

			return true;
		}

		public virtual bool RemoveChildNode(NodeBase node)
		{
			if (childNodes == null) return false;

			NodeBase nodeTemp = childNodes.Find(s => s == node);
			if (nodeTemp == null) return false;

			nodeTemp.parentNode = null;

			return childNodes.Remove(node);
		}

		public virtual void ClearChildNode()
		{
			if (childNodes == null) return;

			childNodes.ForEach(node =>
			{
				node.parentNode = null;
			});
			childNodes.Clear();
		}

		public virtual int GetChildrenCount()
		{
			if (childNodes == null) return 0;
			return childNodes.Count;
		}

		protected virtual IEnumerator RunningRoutine()
		{
			nodeState = NodeState.Running;
			var enumrator = childNodes.GetEnumerator();
			while(enumrator.MoveNext())
			{
				yield return enumrator.Current.RunningRoutine();

				if (enumrator.Current.nodeState == NodeState.Running)
					break;
			}
			nodeState = NodeState.None;
		}
	}
}
