using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviorTree
{
    [CustomEditor(typeof(BehaviorTreeAsset))]
    public class BehaviorTreeAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (targets.Length == 1)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Editor"))
                {
                    BehaviorTreeEditor.OpenWithSelect(target);
                }
            }
        }
    }

}
