using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace BehaviorTree
{
	public class BehaviorTree : MonoBehaviour
	{
		protected NodeBase _rootNode = null;
		public NodeBase rootNode
		{
			protected set
			{
				_rootNode = value;
			}
			get
			{
				return _rootNode;
			}
		}

		protected bool _isRunning = false;
		public bool isRunning
		{
			protected set
			{
				_isRunning = value;
			}
			get
			{
				return _isRunning;
			}
		}

		Coroutine runRoutine;


		public void GenerateNode(XmlDocument xml)
		{
			rootNode = null;
			GenerateNodeByXML(xml.FirstChild, rootNode);
		}

		void GenerateNodeByXML(XmlNode xmlNode, NodeBase btNode)
		{
			var enumrator = xmlNode.GetEnumerator();
			XmlNode xmlNodeTemp;
			while(enumrator.MoveNext())
			{
				if (enumrator.Current == null) continue;
				xmlNodeTemp = enumrator.Current as XmlNode;

				System.Type nodeType = System.Type.GetType(string.Format("BehaviorTree.Node{0}", xmlNodeTemp.Name));
				if (nodeType == null) continue;

				var attrName = xmlNodeTemp.Attributes["Name"];

				NodeBase newBTNode = System.Activator.CreateInstance(nodeType, attrName == null ? "None" : attrName.Value, this) as NodeBase;
				if (newBTNode == null) continue;

				btNode.AddChild(newBTNode);
				GenerateNodeByXML(xmlNodeTemp, newBTNode);
			}
		}

		public void Run()
		{
			if (isRunning) return;
			if (runRoutine != null) StopCoroutine(runRoutine);
			runRoutine = StartCoroutine(RunRoutine());
		}

		public void Stop()
		{
			isRunning = false;
			if (runRoutine != null) StopCoroutine(runRoutine);
		}

		protected IEnumerator RunRoutine()
		{
			isRunning = true;
			while(rootNode != null)
			{
				yield return StartCoroutine(rootNode.RunningRoutine());
			}
			isRunning = false;
		}
	}
}
