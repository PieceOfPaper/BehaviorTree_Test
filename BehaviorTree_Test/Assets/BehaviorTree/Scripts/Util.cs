using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

		public static object Parse(Type type, string str)
		{
			if (type == null) return null;

			if (type == typeof(string))
			{
				return str;
			}
			else if (type.IsEnum)
            {
				return Enum.Parse(type, str);
            }
			else
			{
				MethodInfo parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
				if (parseMethod == null) return null;

				return parseMethod.Invoke(type, new object[] { str });
			}
		}
	}
}
