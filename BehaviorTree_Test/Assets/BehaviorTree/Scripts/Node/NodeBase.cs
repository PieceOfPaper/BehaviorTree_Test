﻿using System.Collections;
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

	public abstract class NodeBase
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
		protected NodeBase[] cachedChildNodes = null;
		protected bool isChildNodesChanged = true;

		protected BehaviorTree baseTree;

		protected NodeState _nodeState = NodeState.None;
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


		//Attributes Variant
		protected string _nodeName;
		public string nodeName
		{
			protected set
			{
				_nodeName = value;
			}
			get
			{
				return _nodeName;
			}
		}

		protected bool _isReverse;
		public virtual bool isReverse
		{
			protected set
			{
				_isReverse = value;
			}
			get
			{
				return _isReverse;
			}
		}

		public NodeBase() { }

		public NodeBase(string nodeName)
		{
			this.nodeName = nodeName;
		}

		public NodeBase(string nodeName, BehaviorTree baseTree)
		{
			this.nodeName = nodeName;
			this.baseTree = baseTree;
		}

		public NodeBase(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
		{
			this.nodeName = xmlAttributes["Name"] != null ? 
				xmlAttributes["Name"].Value : "none";
			this.isReverse = 
				xmlAttributes["IsReverse"] != null &&  xmlAttributes["IsReverse"].Value.ToLower().Contains("true") ? 
				true : false;

			this.baseTree = baseTree;
		}

		public bool AddChild(NodeBase node, int index = -1)
		{
			if (childNodes == null) return false;
			//if (childNodes.Contains(node)) return false;

			if (index == -1) childNodes.Add(node);
			else childNodes.Insert(index, node);

			node.parentNode = this;
			node.baseTree = this.baseTree;
			isChildNodesChanged = true;

			return true;
		}

		public bool RemoveChild(NodeBase node)
		{
			if (childNodes == null) return false;

			NodeBase nodeTemp = childNodes.Find(s => s == node);
			if (nodeTemp == null) return false;

			nodeTemp.parentNode = null;
			isChildNodesChanged = true;

			return childNodes.Remove(node);
		}

		public void ClearChildren()
		{
			if (childNodes == null) return;

			childNodes.ForEach(node =>
			{
				node.parentNode = null;
				node.baseTree = null;
				node.ClearChildren();
			});
			childNodes.Clear();
		}

		public int GetChildrenCount()
		{
			if (childNodes == null) return 0;
			return childNodes.Count;
		}

		public NodeBase[] GetAllChildren()
		{
			if (childNodes == null) return null;
			if (isChildNodesChanged)
			{
				cachedChildNodes = childNodes.ToArray();
				isChildNodesChanged = false;
			}
			return cachedChildNodes;
		}

		public void ResetChildrenState()
		{
			if (childNodes == null) return;
			childNodes.ForEach(node =>
			{
				node.nodeState = NodeState.None;
			});
		}



		public abstract IEnumerator RunningRoutine();
	}
}
