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

	// 필수와 옵션을 나누기위해 만들었는데, 오히려 개발이 불편해짐.
	// 현재는 그냥 대충 Required면 에러정도 띄워주는 정도 처리함.
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

		public NodeAttribute(string name)
        {
			m_Name = name;
			m_Option = NodeAttributeOptionType.Optional;
		}

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

		public virtual NodeState State { protected set; get; } = NodeState.None;


		//Attributes Variant
		[NodeAttribute("Name", NodeAttributeOptionType.Optional)] protected string m_Name;
		[NodeAttribute("IsReverse", NodeAttributeOptionType.Optional)] protected bool m_IsReverse;
		public string Name => m_Name;
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

		public NodeBase GetChild(int index)
		{
			if (childNodes == null) return null;
			if (index < 0 || index >= childNodes.Count) return null;
			return childNodes[index];
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
				node.State = NodeState.None;
			});
		}

		public int ChildIndexOf(NodeBase node)
		{
			if (childNodes == null) return -1;
			return childNodes.IndexOf(node);
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

				if (xmlAttributes[nodeAttr.Name] == null ||
					string.IsNullOrEmpty(xmlAttributes[nodeAttr.Name].Value) == true)
                {
					if (nodeAttr.Option == NodeAttributeOptionType.Required)
						Debug.LogErrorFormat("[BehaviorTree.{0}] Require Attribute - {1}", GetType().Name, nodeAttr.Name);
					continue;
                }

				fields[i].SetValue(this, Util.Parse(fields[i].FieldType, xmlAttributes[nodeAttr.Name].Value));
			}

			this.baseTree = baseTree;
		}

		public abstract IEnumerator RunningRoutine();
	}
}
