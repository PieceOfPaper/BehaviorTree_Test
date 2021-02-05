using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class NodeActionBase : NodeBase
	{
		public NodeActionBase(string nodeName) : base(nodeName) { }
		public NodeActionBase(string nodeName, BehaviorTree baseTree) : base(nodeName) { }
		public NodeActionBase(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree) : base(xmlAttributes, baseTree) { }
	}
}
