using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace BehaviorTree
{
	public class BehaviorTree : MonoBehaviour
	{
		[SerializeField] TextAsset m_XMLFile;
		[SerializeField] bool m_RunOnEnabled = true;



		protected static Dictionary<string, Type> m_CachedNodeTypes = new Dictionary<string, Type>();


		public bool IsRunning => m_IsRunning;


		protected NodeBase m_RootNode = null;
		protected bool m_IsRunning = false;
		protected Coroutine m_RunRoutine;


        private void Awake()
        {
			if (m_XMLFile != null)
			{
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(m_XMLFile.text);
				GenerateNode(xml);
			}
		}

        private void OnEnable()
        {
			if (m_RunOnEnabled == true)
				Run();
        }

        private void OnDisable()
        {
			Stop();
		}


        public void GenerateNode(XmlDocument xml)
		{
			m_RootNode = null;
			GenerateNodeByXML(xml.FirstChild, m_RootNode);
		}

		void GenerateNodeByXML(XmlNode xmlNode, NodeBase btNode)
		{
			var enumrator = xmlNode.GetEnumerator();
			XmlNode xmlNodeTemp;
			while(enumrator.MoveNext())
			{
				if (enumrator.Current == null) continue;
				xmlNodeTemp = enumrator.Current as XmlNode;

				Type nodeType = GetNodeType(xmlNodeTemp.Name);
				if (nodeType == null) continue;

				NodeBase newBTNode = Activator.CreateInstance(nodeType, xmlNodeTemp.Attributes, this) as NodeBase;
				if (newBTNode == null) continue;

				btNode.AddChild(newBTNode);
				GenerateNodeByXML(xmlNodeTemp, newBTNode);
			}
		}

		Type GetNodeType(string nodeTypeName)
        {
			if (m_CachedNodeTypes.ContainsKey(nodeTypeName) == false)
				m_CachedNodeTypes.Add(nodeTypeName, Type.GetType(string.Format("BehaviorTree.Node{0}", nodeTypeName)));
			return m_CachedNodeTypes[nodeTypeName];
		}



		public void Run()
		{
			if (m_IsRunning) return;
			if (m_RootNode == null) return;
			if (m_RunRoutine != null) StopCoroutine(m_RunRoutine);
			m_RunRoutine = StartCoroutine(RunRoutine());
		}

		public void Stop()
		{
			m_IsRunning = false;
			if (m_RunRoutine != null) StopCoroutine(m_RunRoutine);
		}

		IEnumerator RunRoutine()
		{
			m_IsRunning = true;
			while(m_RootNode != null)
			{
				yield return StartCoroutine(m_RootNode.RunningRoutine());
			}
			m_IsRunning = false;
		}
	}
}
