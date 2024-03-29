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
		static Dictionary<Type, MethodInfo> m_CachedParseMethods = new Dictionary<Type, MethodInfo>();


		public static NodeBase GenerateNodeByXml(in TextAsset textAsset)
		{
			return GenerateNodeByXml(null, textAsset);
		}

		public static NodeBase GenerateNodeByXml(in BehaviorTree baseTree, in TextAsset textAsset)
		{
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(textAsset.text);
			return GenerateNodeByXml(baseTree, xml);
		}

		public static NodeBase GenerateNodeByXml(in XmlDocument xml)
		{
			return GenerateNodeByXml(null, xml);
		}

		public static NodeBase GenerateNodeByXml(in BehaviorTree baseTree, in XmlDocument xml)
		{
			var rootXmlNode = xml.LastChild;
			var rootNode = new NodeRoot();
			rootNode.Setup(rootXmlNode.Attributes, baseTree);
			GenerateNodeByXmlRecusively(baseTree, rootXmlNode, rootNode);
			return rootNode;
		}

		static void GenerateNodeByXmlRecusively(BehaviorTree baseTree, XmlNode xmlNode, NodeBase btNode)
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
				GenerateNodeByXmlRecusively(baseTree, xmlNodeTemp, newBTNode);
			}
		}

		public static XmlDocument ConvertToXml(this NodeBase node)
        {
			XmlDocument xml = new XmlDocument();
    		var xmldecl = xml.CreateXmlDeclaration("1.0", "utf-8", null);
			xml.AppendChild(xmldecl);
			ConvertToXmlRecusively(xml, xml, node);
			return xml;
		}

		static void ConvertToXmlRecusively(XmlDocument xmlDocument, XmlNode xmlNode, NodeBase node)
		{
			var type = node.GetType();
			XmlElement newXmlNode = null;

			if (node is NodeRoot)
				newXmlNode = xmlDocument.CreateElement("BehaviorTree");
			else
				newXmlNode = xmlDocument.CreateElement(type.Name.Substring(4, node.GetType().Name.Length - 4));

			var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			for (int i = 0; i < fields.Length; i++)
			{
				object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
				if (attributes == null || attributes.Length == 0) continue;

				NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
				if (nodeAttr == null) continue;

				object fieldValue = fields[i].GetValue(node);
				if (fieldValue == null) continue;

				XmlAttribute xmlAttr = xmlDocument.CreateAttribute(nodeAttr.Name);
				xmlAttr.Value = fieldValue.ToString();
				newXmlNode.Attributes.Append(xmlAttr);
			}
			xmlNode.AppendChild(newXmlNode);

			var childNodes = node.GetAllChildren();
			for (int i = 0; i < childNodes.Length; i ++)
            {
				if (childNodes[i] == null) continue;
				ConvertToXmlRecusively(xmlDocument, newXmlNode, childNodes[i]);
			}
		}



		public static NodeBase GenerateNodeByAsset(in BehaviorTreeAsset asset)
        {
			return GenerateNodeByAsset(null, asset);
		}

		public static NodeBase GenerateNodeByAsset(in BehaviorTree baseTree, in BehaviorTreeAsset asset)
		{
			var noParentNodeDatas = asset.GetNodesByParentID(-1);
			var rootNodeData = noParentNodeDatas == null || noParentNodeDatas.Length == 0 ? null : noParentNodeDatas[0];
			if (rootNodeData == null) return null;

			var rootNode = new NodeRoot();
			rootNode.Setup(rootNodeData.Attributes, baseTree);
			GenerateNodeByAssetRecusively(baseTree, asset, rootNodeData, rootNode);
			return rootNode;
		}

		static void GenerateNodeByAssetRecusively(BehaviorTree baseTree, BehaviorTreeAsset asset, SerializedNode nodeData, NodeBase btNode)
		{
			if (nodeData == null) return;

			var children = asset.GetNodesByParentID(nodeData.ID);
			for (int i = 0; i < children.Length; i ++)
            {
				if (children[i] == null) continue;

				Type nodeType = GetNodeType(children[i].Type);
				if (nodeType == null) continue;

				NodeBase newBTNode = Activator.CreateInstance(nodeType) as NodeBase;
				if (newBTNode == null) continue;

				newBTNode.Setup(children[i].Attributes, baseTree);
				btNode.AddChild(newBTNode);
				GenerateNodeByAssetRecusively(baseTree, asset, children[i], newBTNode);
			}
		}

		public static SerializedNode[] ConvertToAsset(this NodeBase node)
		{
			List<SerializedNode> list = new List<SerializedNode>();
			SerializedNode nodeData = new SerializedNode(list.Count, -1, "Root");
			list.Add(nodeData);
			var childNodes = node.GetAllChildren();
			for (int i = 0; i < childNodes.Length; i++)
			{
				if (childNodes[i] == null) continue;
				ConvertToAssetRecusively(nodeData, childNodes[i], list);
			}
			return list.ToArray();
		}

		static void ConvertToAssetRecusively(SerializedNode nodeData, NodeBase node, List<SerializedNode> list)
		{
			var type = node.GetType();
			SerializedNode newNodeData = null;

			newNodeData = new SerializedNode(list.Count, nodeData.ID, type.Name.Substring(4, node.GetType().Name.Length - 4));
			list.Add(newNodeData);

			newNodeData.ModifyAttributes((list) =>
			{
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				for (int i = 0; i < fields.Length; i++)
				{
					object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
					if (attributes == null || attributes.Length == 0) continue;

					NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
					if (nodeAttr == null) continue;

					object fieldValue = fields[i].GetValue(node);
					if (fieldValue == null) continue;

					list.Add(new SerializedNodeAttribute(nodeAttr.Name, fieldValue.ToString()));
				}
			});

			var childNodes = node.GetAllChildren();
			for (int i = 0; i < childNodes.Length; i++)
			{
				if (childNodes[i] == null) continue;
				ConvertToAssetRecusively(newNodeData, childNodes[i], list);
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
				MethodInfo parseMethod = null;

				if (m_CachedParseMethods.ContainsKey(type) == false)
					m_CachedParseMethods.Add(type, type.GetMethod("Parse", new Type[] { typeof(string) }));

				parseMethod = m_CachedParseMethods[type];
				if (parseMethod == null) return null;

				return parseMethod.Invoke(type, new object[] { str });
			}
		}
	}
}
