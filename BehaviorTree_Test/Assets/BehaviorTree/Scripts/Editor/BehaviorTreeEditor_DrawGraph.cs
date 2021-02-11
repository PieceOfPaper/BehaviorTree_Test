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
        const float GRAPH_WINDOW_NAME_WIDTH = 100;
        const float GRAPH_WINDOW_VALUE_WIDTH = 100;
        const float GRAPH_SPACE_X = 50;
        const float GRAPH_SPACE_Y = 50;

        static string[] ADDABLE_NODE_TYPES = new string[] {
            "Add",

            "Selector",
            "Sequence",

            "Action_SendMessage",
            "Action_AnimatorState",
            "Action_Wait",

            "Condition_ComponentField",
        };

        Dictionary<string, object> m_CopiedAttributes = new Dictionary<string, object>();

        public void DrawGraph(NodeBase rootNode, bool isEditable = false)
        {
            GUILayout.Space(10);
            DrawGraphRecusively(rootNode, isEditable);
        }

        void DrawGraphRecusively(NodeBase node, bool isEditable = false)
        {
            EditorGUILayout.BeginVertical();
            {
                Rect parentRect;
                var children = node.GetAllChildren();
                var fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                Dictionary<string, object> changedValues = new Dictionary<string, object>();
                

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                {
                    Color defaultColor = GUI.color;
                    GUILayout.BeginVertical(node.GetType().Name.Substring(4, node.GetType().Name.Length - 4), "window", GUILayout.Width(200));
                    
                    switch(node.State)
                    {
                        case NodeState.Running:
                            GUI.color = Color.yellow;
                            break;
                        case NodeState.Success:
                            GUI.color = Color.green;
                            break;
                        case NodeState.Fail:
                            GUI.color = Color.red;
                            break;
                    }

                    if (node.parentNode != null && isEditable == true)
                    {
                        var parentNode = node.parentNode;
                        var nodeIndex = parentNode.ChildIndexOf(node);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("◀") && 
                            nodeIndex > 0)
                        {
                            parentNode.RemoveChild(node);
                            parentNode.AddChild(node, nodeIndex - 1);
                        }
                        if (GUILayout.Button("▲") && 
                            parentNode.parentNode != null)
                        {
                            parentNode.RemoveChild(node);
                            parentNode.parentNode.AddChild(node);
                        }
                        if (GUILayout.Button("▼") && 
                            nodeIndex > 0 && parentNode.GetChildrenCount() >= 2)
                        {
                            parentNode.RemoveChild(node);
                            parentNode.GetChild(nodeIndex - 1).AddChild(node);
                        }
                        if (GUILayout.Button("▶") && 
                            nodeIndex < parentNode.GetChildrenCount() - 1)
                        {
                            parentNode.RemoveChild(node);
                            parentNode.AddChild(node, nodeIndex + 1);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Box("", GUILayout.ExpandWidth(true));
                    }

                    if (isEditable == true)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Copy"))
                        {
                            m_CopiedAttributes.Clear();
                            for (int i = 0; i < fields.Length; i ++)
                            {
                                object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
                                if (attributes == null || attributes.Length == 0) continue;

                                NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
                                if (nodeAttr == null) continue;
                                if (nodeAttr.Option != NodeAttributeOptionType.Required) continue;

                                m_CopiedAttributes.Add(nodeAttr.Name, fields[i].GetValue(node));
                            }
                        }
                        if (GUILayout.Button("Paste"))
                        {
                            for (int i = 0; i < fields.Length; i ++)
                            {
                                object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
                                if (attributes == null || attributes.Length == 0) continue;

                                NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
                                if (nodeAttr == null) continue;
                                if (m_CopiedAttributes.ContainsKey(nodeAttr.Name) == false) continue;

                                fields[i].SetValue(node, m_CopiedAttributes[nodeAttr.Name]);
                            }
                        }
                        if (GUILayout.Button("Delete") && node.parentNode != null)
                        {
                            var parentNode = node.parentNode;
                            parentNode.RemoveChild(node);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Box("", GUILayout.ExpandWidth(true));
                    }

                    if (node is NodeRoot)
                    {
                        GUILayout.Label("", GUILayout.Width(GRAPH_WINDOW_NAME_WIDTH + GRAPH_WINDOW_VALUE_WIDTH));
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Name", GUILayout.Width(GRAPH_WINDOW_NAME_WIDTH));
                        var changedName = EditorGUILayout.TextField(node.Name, GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        if (changedName != node.Name) changedValues.Add("Name", changedName);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("State", GUILayout.Width(GRAPH_WINDOW_NAME_WIDTH));
                    GUILayout.Label(node.State.ToString(), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(10);

                    for (int i = 0; i < fields.Length; i ++)
                    {
                        object[] attributes = fields[i].GetCustomAttributes(typeof(NodeAttribute), true);
                        if (attributes == null || attributes.Length == 0) continue;

                        NodeAttribute nodeAttr = attributes[0] as NodeAttribute;
                        if (nodeAttr == null) continue;
                        if (nodeAttr.Name == "Name") continue;
                        if (nodeAttr.Option != NodeAttributeOptionType.Required) continue;

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(nodeAttr.Name, GUILayout.Width(GRAPH_WINDOW_NAME_WIDTH));
                        object changedValue = null;
                        if (fields[i].FieldType == typeof(string))
                            changedValue = EditorGUILayout.TextField((string)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else if (fields[i].FieldType == typeof(int))
                            changedValue = EditorGUILayout.IntField((int)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else if (fields[i].FieldType == typeof(long))
                            changedValue = EditorGUILayout.LongField((long)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else if (fields[i].FieldType == typeof(float))
                            changedValue = EditorGUILayout.FloatField((float)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else if (fields[i].FieldType == typeof(double))
                            changedValue = EditorGUILayout.DoubleField((double)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else if (fields[i].FieldType == typeof(bool))
                            changedValue = EditorGUILayout.Toggle((bool)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else if (fields[i].FieldType.IsEnum)
                            changedValue = EditorGUILayout.EnumPopup((Enum)fields[i].GetValue(node), GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        else
                            GUILayout.Label(fields[i].FieldType.Name, GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
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


                    if (isEditable == true)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        var addSelectedIndex = EditorGUILayout.Popup(0, ADDABLE_NODE_TYPES, GUILayout.Width(GRAPH_WINDOW_VALUE_WIDTH));
                        if (addSelectedIndex > 0)
                        {
                            Type nodeType = Util.GetNodeType(ADDABLE_NODE_TYPES[addSelectedIndex]);
                            if (nodeType != null)
                            {
                                NodeBase newNode = Activator.CreateInstance(nodeType) as NodeBase;
                                node.AddChild(newNode);
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Box("", GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndVertical();
                    parentRect = GUILayoutUtility.GetLastRect();
                    GUI.color = defaultColor;
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();


                if (isEditable == true &&
                    changedValues.Count > 0)
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

                if (children.Length > 0)
                {
                    GUILayout.Space(GRAPH_SPACE_Y - 10);
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(10));

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box("", GUILayout.Width(10), GUILayout.ExpandHeight(true));
                    GUILayout.Space(GRAPH_SPACE_X - 10);
                    for (int i = 0; i < children.Length; i ++)
                    {
                        if (i > 0) GUILayout.Space(GRAPH_SPACE_X);
                        DrawGraphRecusively(children[i], isEditable);
                        Rect childRect = GUILayoutUtility.GetLastRect();
                        Vector3 startPos = new Vector3(
                                parentRect.center.x, 
                                parentRect.center.y + parentRect.height * 0.5f, 
                                0);
                        Vector3 endPos = new Vector3(
                                childRect.center.x, 
                                childRect.center.y - childRect.height * 0.5f, 
                                0);
                        if (EditorGUIUtility.isProSkin == false) //outline
                        {
                            Handles.DrawBezier(startPos, endPos,
                            new Vector3(startPos.x, endPos.y, 0),
                            new Vector3(endPos.x, startPos.y, 0),
                            Color.black,
                            null, 4f);
                        }
                        Handles.DrawBezier(startPos, endPos,
                            new Vector3(startPos.x, endPos.y, 0),
                            new Vector3(endPos.x, startPos.y, 0),
                            children[i].State == NodeState.Running ? Color.yellow : Color.white,
                            null, 2f);
                    }
                    GUILayout.Space(GRAPH_SPACE_X - 10);
                    GUILayout.Box("", GUILayout.Width(10), GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
