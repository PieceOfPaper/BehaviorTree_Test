using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeGenerateEditor : EditorWindow
{
	public static readonly string[] BASE_NODES = new string[]
	{
		"Default",

		"Action",
		"Condition",
	};


	[MenuItem("BehaviorTree/Open Node Generate Editor")]
	public static NodeGenerateEditor Open()
	{
		NodeGenerateEditor window = (NodeGenerateEditor)EditorWindow.GetWindow(typeof(NodeGenerateEditor), true, "Node Generate Editor");
		window.Show();
		return window;
	}

	public static void Generate(string typeName, int baseNodeIndex)
	{
		string saveFileNameForm = null;
		string baseFileName = null;
		switch(baseNodeIndex)
        {
			case 1:
				saveFileNameForm = "NodeAction_{0}.cs";
				baseFileName = "NodeActionBase.txt"; 
				break;
			case 2:
				saveFileNameForm = "NodeCondition_{0}.cs";
				baseFileName = "NodeConditionBase.txt"; 
				break;
			default:
				saveFileNameForm = "Node{0}.cs";
				baseFileName = "NodeBase.txt"; 
				break;
		}

		TextAsset baseText = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/BehaviorTree/Scripts/Editor/NodeForm/{baseFileName}");
		if (baseText == null) return;

		var savePath = EditorUtility.SaveFilePanel(
			"Generate Node",
			Application.dataPath,
			string.Format(saveFileNameForm, typeName),
			"cs");

		if (string.IsNullOrEmpty(savePath) == false)
		{
			if (savePath.StartsWith(Application.dataPath) == true)
			{
				System.IO.File.WriteAllText(savePath, baseText.text.Replace("%%TYPENAME%%", typeName));
				AssetDatabase.Refresh();
			}
			else
			{
				Debug.LogError("Save Path Error");
			}
		}
	}


	string TargetTypeName
	{
		get { return EditorPrefs.GetString("NodeGenerateEditor_TargetTypeName", string.Empty); }
		set { EditorPrefs.SetString("NodeGenerateEditor_TargetTypeName", value); }
	}

	int TargetBaseNodeIndex
	{
		get { return EditorPrefs.GetInt("NodeGenerateEditor_TargetBaseNodeIndex", 0); }
		set { EditorPrefs.SetInt("NodeGenerateEditor_TargetBaseNodeIndex", value); }
	}


	private void OnGUI()
    {
		TargetTypeName = EditorGUILayout.TextField("Name", TargetTypeName);
		TargetBaseNodeIndex = EditorGUILayout.Popup("Base Type", TargetBaseNodeIndex, BASE_NODES);

		GUILayout.Space(30);

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Generate!"))
        {
			Generate(TargetTypeName, TargetBaseNodeIndex);
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}
}
