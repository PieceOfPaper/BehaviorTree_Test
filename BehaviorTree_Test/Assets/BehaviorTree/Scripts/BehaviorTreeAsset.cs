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
        [SerializeField] int m_ID;
        public int ID => m_ID;

        [SerializeField] int m_ParentID;
        public int ParentID => m_ParentID;

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

        public SerializedNode(int myID, int parentID, string type)
        {
            m_ID = myID;
            m_ParentID = parentID;
            m_Type = type;
        }
    }


    public class BehaviorTreeAsset : ScriptableObject
    {
        [SerializeField] SerializedNode[] m_Nodes;
        public SerializedNode[] Nodes => m_Nodes;
#if UNITY_EDITOR
        public void SetNodes(SerializedNode[] nodes) => m_Nodes = nodes;
#endif

        public SerializedNode GetNodeByID(int ID)
        {
            return Array.Find(m_Nodes, m => m.ID == ID);
        }

        public SerializedNode[] GetNodesByParentID(int parentID)
        {
            if (m_Nodes == null) return null;
            return Array.FindAll(m_Nodes, m => m.ParentID == parentID);
        }
    }
}
