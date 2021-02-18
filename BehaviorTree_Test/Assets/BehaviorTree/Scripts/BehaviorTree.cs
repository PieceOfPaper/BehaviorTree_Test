using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace BehaviorTree
{
	public class BehaviorTree : MonoBehaviour
	{
		[SerializeField] UnityEngine.Object m_TargetObject = null;


		public TextAsset XmlFile
        {
			set { m_TargetObject = value; LoadData(); }
			get { return m_TargetObject as TextAsset; }
		}
		public BehaviorTreeAsset AssetFile
		{
			set { m_TargetObject = value; LoadData(); }
			get { return m_TargetObject as BehaviorTreeAsset; }
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
			LoadData();
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


		public void LoadData()
        {
			if (Application.isPlaying == false) return;

			if (XmlFile != null)
				m_RootNode = Util.GenerateNodeByXml(this, XmlFile);
			else if (AssetFile != null)
				m_RootNode = Util.GenerateNodeByAsset(this, AssetFile);
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
