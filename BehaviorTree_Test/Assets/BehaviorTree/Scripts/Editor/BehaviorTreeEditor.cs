using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

namespace BehaviorTree
{
	public partial class BehaviorTreeEditor : EditorWindow
	{
        #region Static Methods

        [MenuItem("BehaviorTree/Open Editor")]
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


		[MenuItem("BehaviorTree/Create New XML")]
		public static string CreateNewXml()
		{
			var savePath = EditorUtility.SaveFilePanel(
				"Create New XML",
				Application.dataPath,
				"",
				"xml");

			if (string.IsNullOrEmpty(savePath) == false)
			{
				if (savePath.StartsWith(Application.dataPath) == true)
				{
					var assetPath = savePath.Replace(Application.dataPath, "Assets");
					var saveSuccess = Save(assetPath, new NodeRoot());
					AssetDatabase.Refresh();
					return saveSuccess ? assetPath : null;
				}
				else
				{
					Debug.LogError("Save Path Error");
				}
			}

			return null;
		}

		[MenuItem("BehaviorTree/Create New Asset")]
		public static string CreateNewAsset()
		{
			var savePath = EditorUtility.SaveFilePanel(
				"Create New Asset",
				Application.dataPath,
				"",
				"asset");

			if (string.IsNullOrEmpty(savePath) == false)
			{
				if (savePath.StartsWith(Application.dataPath) == true)
				{
					var assetPath = savePath.Replace(Application.dataPath, "Assets");
					var saveSuccess = Save(assetPath, new NodeRoot());
					AssetDatabase.Refresh();
					return saveSuccess ? assetPath : null;
				}
				else
				{
					Debug.LogError("Save Path Error");
				}
			}

			return null;
		}


		public static bool Save(Object targetObj, NodeBase rootNode)
		{
			if (targetObj == null) return false;

			string assetPath = null;
			if (targetObj is GameObject)
			{
				var behaviorTree = ((GameObject)targetObj).GetComponent<BehaviorTree>();
				if (behaviorTree != null && EditorApplication.isPlaying == false)
				{
					if (behaviorTree.XmlFile != null)
						assetPath = AssetDatabase.GetAssetPath(behaviorTree.XmlFile);
					else if (behaviorTree.AssetFile != null)
						assetPath = AssetDatabase.GetAssetPath(behaviorTree.AssetFile);
				}
			}
			else if (targetObj is TextAsset || targetObj is BehaviorTreeAsset)
			{
				assetPath = AssetDatabase.GetAssetPath(targetObj);
			}

			return Save(assetPath, rootNode);
		}

		public static bool Save(string assetPath, NodeBase rootNode)
		{
			if (string.IsNullOrEmpty(assetPath) == true) return false;

			var lowerPath = assetPath.ToLower();
			if (lowerPath.EndsWith(".xml"))
			{
				var xml = rootNode.ConvertToXml();
				if (xml == null) return false;

				xml.Save(System.IO.Path.Combine(Application.dataPath, assetPath.Replace("Assets/", "")));
			}
			else if (lowerPath.EndsWith(".asset"))
			{
				var asset = AssetDatabase.LoadAssetAtPath<BehaviorTreeAsset>(assetPath);
				if (asset == null)
                {
					asset = CreateInstance<BehaviorTreeAsset>();
					AssetDatabase.CreateAsset(asset, assetPath);
				}
				asset.SetRootNode(rootNode.ConvertToAsset());
				EditorUtility.SetDirty(asset);
				AssetDatabase.SaveAssets();

			}
			AssetDatabase.Refresh();
			return true;
		}

        #endregion



        #region Editor - Variable

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

		#endregion


		#region Editor - Const

		const float WINDOW_MINSIZE_WIDTH = 200;
		const float WINDOW_MINSIZE_HEIGHT = 200;

		const float MENU_WIDTH = 200;

		#endregion


		#region Editor - Method (OnGUI)

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
						m_RootNode = null;
						Repaint();
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
							else if (behaviorTree.AssetFile != null)
							{
								m_RootNode = Util.GenerateNodeByAsset(behaviorTree.AssetFile);
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
				else if (m_Selection is BehaviorTreeAsset &&
					string.IsNullOrEmpty(assetPath) == false && assetPath.ToLower().EndsWith(".asset"))
				{
					m_RootNode = Util.GenerateNodeByAsset((BehaviorTreeAsset)m_Selection);
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
				var assetPath = CreateNewXml();
				if (string.IsNullOrEmpty(assetPath) == false)
					Select(AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)));
			}
			if (GUILayout.Button("Create New Asset"))
			{
				var assetPath = CreateNewAsset();
				if (string.IsNullOrEmpty(assetPath) == false)
					Select(AssetDatabase.LoadAssetAtPath(assetPath, typeof(BehaviorTreeAsset)));
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

		#endregion


		#region Editor - Method (Public)

		public void Select(Object obj)
        {
			m_Selection = obj;
			m_RootNode = null;
			var assetPath = m_Selection == null ? string.Empty : AssetDatabase.GetAssetPath(m_Selection);
			SelectionAssetGUID = string.IsNullOrEmpty(assetPath) ? string.Empty : AssetDatabase.AssetPathToGUID(assetPath);
        }

        #endregion
    }

}
