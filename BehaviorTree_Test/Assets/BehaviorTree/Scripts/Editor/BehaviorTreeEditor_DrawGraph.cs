using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace BehaviorTree
{
    public partial class BehaviorTreeEditor : EditorWindow
    {
        public void DrawGraph(NodeBase rootNode, bool isEditable = false)
        {
            DrawGraphRecusively(rootNode, isEditable);
        }

        void DrawGraphRecusively(NodeBase node, bool isEditable = false)
        {
            EditorGUILayout.BeginVertical();
            {
                var fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                Dictionary<string, object> changedValues = new Dictionary<string, object>();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                {
                    GUILayout.BeginVertical(node.GetType().Name.Substring(4, node.GetType().Name.Length - 4).Replace('_', ' '), "window", GUILayout.Width(200));
                    
                    if (node is NodeRoot)
                    {
                        GUILayout.Label("", GUILayout.Width(100));
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Name", GUILayout.Width(100));
                        var changedName = EditorGUILayout.TextField(node.Name, GUILayout.Width(100));
                        if (changedName != node.Name) changedValues.Add("Name", changedName);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(10);
                    }

                    for (int i = 0; i < fields.Length; i ++)
                    {
                        object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
                        if (attributes == null || attributes.Length == 0) continue;

                        NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
                        if (nodeAttr == null) continue;
                        if (nodeAttr.Name == "Name") continue; // 얘는 따로 그려주자.
                        if (nodeAttr.Option != NodeAttributeOptionType.Required) continue;

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(nodeAttr.Name, GUILayout.Width(100));
                        object changedValue = null;
                        if (fields[i].FieldType == typeof(string))
                            changedValue = EditorGUILayout.TextField((string)fields[i].GetValue(node), GUILayout.Width(100));
                        else if (fields[i].FieldType == typeof(int))
                            changedValue = EditorGUILayout.IntField((int)fields[i].GetValue(node), GUILayout.Width(100));
                        else if (fields[i].FieldType == typeof(long))
                            changedValue = EditorGUILayout.LongField((long)fields[i].GetValue(node), GUILayout.Width(100));
                        else if (fields[i].FieldType == typeof(float))
                            changedValue = EditorGUILayout.FloatField((float)fields[i].GetValue(node), GUILayout.Width(100));
                        else if (fields[i].FieldType == typeof(double))
                            changedValue = EditorGUILayout.DoubleField((double)fields[i].GetValue(node), GUILayout.Width(100));
                        else if (fields[i].FieldType == typeof(bool))
                            changedValue = EditorGUILayout.Toggle((bool)fields[i].GetValue(node), GUILayout.Width(100));
                        else if (fields[i].FieldType.IsEnum)
                            changedValue = EditorGUILayout.EnumPopup((Enum)fields[i].GetValue(node), GUILayout.Width(100));
                        else
                            GUILayout.Label(fields[i].FieldType.Name, GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();

                        if (changedValue != null)
                        {
                            if (changedValue is IComparable)
                            {
                                if (((IComparable)fields[i].GetValue(node)).CompareTo(changedValue) != 0)
                                    changedValues.Add(nodeAttr.Name, changedValue);
                            }
                            else
                            {
                                if (fields[i].GetValue(node) != changedValue)
                                    changedValues.Add(nodeAttr.Name, changedValue);
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (changedValues.Count > 0)
                {
                    for (int i = 0; i < fields.Length; i ++)
                    {
                        object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
                        if (attributes == null || attributes.Length == 0) continue;

                        NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
                        if (nodeAttr == null) continue;
                        if (changedValues.ContainsKey(nodeAttr.Name) == false) continue;

                        fields[i].SetValue(node, changedValues[nodeAttr.Name]);
                    }
                }

                GUILayout.Space(50);

                var children = node.GetAllChildren();
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < children.Length; i ++)
                {
                    GUILayout.Space(50);
                    DrawGraphRecusively(children[i], isEditable);
                    GUILayout.Space(50);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
