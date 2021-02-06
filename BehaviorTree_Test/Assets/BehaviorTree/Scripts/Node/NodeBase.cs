using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

	public enum NodeAttributeOptionType
    {
		Required,
		Optional,
    }

	[AttributeUsage(AttributeTargets.Field)]
	public class NodeAttribute : Attribute
	{
		string m_Name;
		NodeAttributeOptionType m_Option;

		public string Name => m_Name;
		public NodeAttributeOptionType Option => m_Option;

		public NodeAttribute(string name, NodeAttributeOptionType option)
        {
			m_Name = name;
			m_Option = option;

		}
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
		[NodeAttribute("Name", NodeAttributeOptionType.Optional)] protected string m_Name;
		[NodeAttribute("IsReverse", NodeAttributeOptionType.Optional)] protected bool m_IsReverse;
		public bool IsReverse => m_IsReverse;


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


		public virtual void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
		{
			var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			for (int i = 0; i < fields.Length; i ++)
			{
				object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
				if (attributes == null || attributes.Length == 0) continue;

				NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
				if (nodeAttr == null) continue;

				if (xmlAttributes[nodeAttr.Name] == null) continue;
				if (string.IsNullOrEmpty(xmlAttributes[nodeAttr.Name].Value) == true) continue;

				fields[i].SetValue(this, Util.Parse(fields[i].FieldType, xmlAttributes[nodeAttr.Name].Value));
			}

			this.baseTree = baseTree;
		}

		public abstract IEnumerator RunningRoutine();
	}
}
