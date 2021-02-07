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
		public TextAsset XmlFile
        {
			set { m_XMLFile = value; LoadXml(); }
			get { return m_XMLFile; }
        }

		[SerializeField] bool m_RunOnEnabled = true;
		public bool RunOnEnabled => m_RunOnEnabled;



		public NodeBase RootNode => m_RootNode;
		public bool IsRunning => m_IsRunning;


		protected NodeBase m_RootNode = null;
		protected bool m_IsRunning = false;
		protected Coroutine m_RunRoutine;


        private void Awake()
        {
			LoadXml();
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


		public void LoadXml()
        {
			if (Application.isPlaying == false) return;
			if (m_XMLFile == null) return;

			m_RootNode = Util.GenerateNodeByXml(this, m_XMLFile);
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
