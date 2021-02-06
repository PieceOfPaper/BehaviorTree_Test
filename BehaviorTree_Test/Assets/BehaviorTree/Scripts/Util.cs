using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorTree
{
    public static class Util
	{
		static Dictionary<string, Type> m_CachedNodeTypes = new Dictionary<string, Type>();



		public static void GenerateNodeByXML(BehaviorTree baseTree, XmlNode xmlNode, NodeBase btNode)
		{
			var enumrator = xmlNode.GetEnumerator();
			XmlNode xmlNodeTemp;
			while (enumrator.MoveNext())
			{
				if (enumrator.Current == null) continue;
				xmlNodeTemp = enumrator.Current as XmlNode;

				Type nodeType = GetNodeType(xmlNodeTemp.Name);
				if (nodeType == null) continue;

				NodeBase newBTNode = Activator.CreateInstance(nodeType) as NodeBase;
				if (newBTNode == null) continue;

				newBTNode.Setup(xmlNodeTemp.Attributes, baseTree);
				btNode.AddChild(newBTNode);
				GenerateNodeByXML(baseTree, xmlNodeTemp, newBTNode);
			}
		}

		public static Type GetNodeType(string nodeTypeName)
		{
			if (m_CachedNodeTypes.ContainsKey(nodeTypeName) == false)
				m_CachedNodeTypes.Add(nodeTypeName, Type.GetType(string.Format("BehaviorTree.Node{0}", nodeTypeName)));
			return m_CachedNodeTypes[nodeTypeName];
		}
	}
}
