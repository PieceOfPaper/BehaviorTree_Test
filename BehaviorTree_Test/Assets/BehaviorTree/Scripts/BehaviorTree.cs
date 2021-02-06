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
			var rootXmlNode = xml.LastChild;
			m_RootNode = new NodeRoot();
			m_RootNode.Setup(rootXmlNode.Attributes, this);
			Util.GenerateNodeByXML(this, rootXmlNode, m_RootNode);
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
