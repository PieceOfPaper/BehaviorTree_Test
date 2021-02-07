using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviorTree
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeInspector : Editor
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
