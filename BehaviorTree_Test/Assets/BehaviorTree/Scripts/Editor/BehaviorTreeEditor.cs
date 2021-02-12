using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

namespace BehaviorTree
{
	public partial class BehaviorTreeEditor : EditorWindow
	{
		const float WINDOW_MINSIZE_WIDTH = 200;
		const float WINDOW_MINSIZE_HEIGHT = 200;

		const float MENU_WIDTH = 200;



		[MenuItem("BehaviorTree/Editor")]
		public static BehaviorTreeEditor Open()
		{
			BehaviorTreeEditor window = (BehaviorTreeEditor)EditorWindow.GetWindow(typeof(BehaviorTreeEditor), false, "BehaviorTree Editor");
			window.minSize = new Vector2(WINDOW_MINSIZE_WIDTH, WINDOW_MINSIZE_HEIGHT);
			window.Show();
			return window;
		}

		public static BehaviorTreeEditor OpenWithSelect(Object obj)
		{
			var window = Open();
			window.Select(obj);
			window.Repaint();
			return window;
		}




		string SelectionAssetGUID
        {
			get { return EditorPrefs.GetString("BehaviorTreeEditor_SelectionAssetGUID", string.Empty); }
			set { EditorPrefs.SetString("BehaviorTreeEditor_SelectionAssetGUID", value); }
        }

		Object m_Selection = null;
		
		NodeBase m_RootNode = null;
		bool m_IsEditable = false;
		Vector2 m_MenuScroll = Vector2.zero;
		Vector2 m_GraphScroll = Vector2.zero;

		void OnGUI()
		{
			if (CheckSelection() == false)
			{
				m_RootNode = null;
				m_IsEditable = false;
				m_GraphScroll = Vector2.zero;
			}

			EditorGUILayout.BeginHorizontal();
			{
				m_MenuScroll = EditorGUILayout.BeginScrollView(m_MenuScroll, GUILayout.Width(MENU_WIDTH));
				{
					OnGUI_Menu();
				}
				EditorGUILayout.EndScrollView();

				m_GraphScroll = EditorGUILayout.BeginScrollView(m_GraphScroll);
				{
					OnGUI_Graph();
				}
				EditorGUILayout.EndScrollView();

				if (m_IsEditable == true)
				{
					if (GUI.Button(new Rect(position.width - 60, 10, 50, 20), "Save"))
					{
						Save(m_Selection, m_RootNode);
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		bool CheckSelection()
		{
			if (m_RootNode != null) return true;

			string assetPath = null;
			if (string.IsNullOrEmpty(SelectionAssetGUID) == false)
			{
				assetPath = AssetDatabase.GUIDToAssetPath(SelectionAssetGUID);
				if (m_Selection == null)
				{
					m_Selection = AssetDatabase.LoadMainAssetAtPath(assetPath);
					if (m_Selection == null) SelectionAssetGUID = string.Empty;
				}
			}

			if (m_Selection != null)
			{
				if (m_Selection is GameObject || m_Selection is BehaviorTree)
				{
					var behaviorTree = m_Selection is BehaviorTree ?
						(BehaviorTree)m_Selection :
						((GameObject)m_Selection).GetComponent<BehaviorTree>();
					if (behaviorTree != null)
					{
						if (EditorApplication.isPlaying)
						{
							m_RootNode = behaviorTree.RootNode;
							m_IsEditable = false;
							return true;
						}
						else
						{
							if (behaviorTree.XmlFile != null)
							{
								m_RootNode = Util.GenerateNodeByXml(behaviorTree.XmlFile);
								m_IsEditable = true;
								return true;
							}
						}
					}
				}
				else if (m_Selection is TextAsset &&
					string.IsNullOrEmpty(assetPath) == false && assetPath.ToLower().EndsWith(".xml"))
				{
					m_RootNode = Util.GenerateNodeByXml((TextAsset)m_Selection);
					m_IsEditable = true;
					return true;
				}
			}

			return false;
		}

		void OnGUI_Menu()
		{
			EditorGUILayout.BeginVertical(GUILayout.Width(MENU_WIDTH));

			var newSelection = EditorGUILayout.ObjectField(m_Selection, typeof(Object), true, GUILayout.Height(100));
			if (m_Selection != newSelection) Select(newSelection);
			EditorGUILayout.TextField(string.IsNullOrEmpty(SelectionAssetGUID) ? "No GUID" : SelectionAssetGUID);

			EditorGUILayout.Space();

			if (GUILayout.Button("Create New XML"))
            {
				// TO DO - Node이름만 적당히 입력하면 만들어주는 팝업을 따로 만들자.
				// 상속받는 클래스이름, Node이름 두가지정도만 Input으로 받으면 될 것 같다.
			}

			EditorGUILayout.Space();

			EditorGUILayout.EndVertical();
		}

		void OnGUI_Graph()
		{
			if (m_RootNode == null)
			{
				GUILayout.Label("Please, Select BehaviorTree Component or Xml File.");
				return;
			}

			DrawGraph(m_RootNode, m_IsEditable);
		}


		public void Select(Object obj)
        {
			m_Selection = obj;
			m_RootNode = null;
			var assetPath = m_Selection == null ? string.Empty : AssetDatabase.GetAssetPath(m_Selection);
			SelectionAssetGUID = string.IsNullOrEmpty(assetPath) ? string.Empty : AssetDatabase.AssetPathToGUID(assetPath);
        }

		public void Save(Object targetObj, NodeBase rootNode)
        {
			if (targetObj == null) return;

			string assetPath = null;
			if (m_Selection is GameObject)
			{
				var behaviorTree = ((GameObject)m_Selection).GetComponent<BehaviorTree>();
				if (behaviorTree != null && EditorApplication.isPlaying == false)
					assetPath = AssetDatabase.GetAssetPath(behaviorTree.XmlFile);
			}
			else if (m_Selection is TextAsset)
			{
				assetPath = AssetDatabase.GetAssetPath(m_Selection);
			}

			if (string.IsNullOrEmpty(assetPath) == true) return;

			var xml = rootNode.ConvertToXml();
			if (xml == null) return;

			xml.Save(System.IO.Path.Combine(Application.dataPath, assetPath.Replace("Assets/", "")));
			AssetDatabase.Refresh();

			m_RootNode = null;
			Repaint();
		}
	}

}
