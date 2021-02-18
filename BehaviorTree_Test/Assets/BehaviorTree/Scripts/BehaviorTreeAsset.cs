using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [System.Serializable]
    public class SerializedNodeAttribute
    {
        [SerializeField] string m_Name;
        public string Name => m_Name;

        [SerializeField] string m_Value;
        public string Value => m_Value;

        public SerializedNodeAttribute(string name, string value)
        {
            m_Name = name;
            m_Value = value;
        }
    }

    [System.Serializable]
    public class SerializedNode
    {
        [SerializeField] string m_Type;
        public string Type => m_Type;


        [SerializeField] SerializedNodeAttribute[] m_Attributes;
        public SerializedNodeAttribute[] Attributes => m_Attributes;
#if UNITY_EDITOR
        public void ModifyAttributes(Action<IList<SerializedNodeAttribute>> method)
        {
            var list = m_Attributes == null ? new List<SerializedNodeAttribute>() : new List<SerializedNodeAttribute>(m_Attributes);
            method?.Invoke(list);
            m_Attributes = list == null ? null : list.ToArray();
        }
#endif

        [SerializeField] SerializedNode[] m_Children;
        public SerializedNode[] Children => m_Children;
#if UNITY_EDITOR
        public void ModifyChildren(Action<IList<SerializedNode>> method)
        {
            var list = m_Children == null ? new List<SerializedNode>() : new List<SerializedNode>(m_Children);
            method?.Invoke(list);
            m_Children = list == null ? null : list.ToArray();
        }
#endif

        public SerializedNode(string type)
        {
            m_Type = type;
        }
    }


    [CreateAssetMenu(fileName = "BehaviorTree", menuName = "Scriptable Object/BehaviorTree")]
    public class BehaviorTreeAsset : ScriptableObject
    {
        [SerializeField] SerializedNode m_RootNode;
        public SerializedNode RootNode => m_RootNode;
#if UNITY_EDITOR
        public void SetRootNode(SerializedNode node) => m_RootNode = node;
#endif
    }
}
