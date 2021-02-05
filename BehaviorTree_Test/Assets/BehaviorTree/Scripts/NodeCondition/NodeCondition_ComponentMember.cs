using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeCondition_ComponentField : NodeConditionBase
	{
		Type m_TargetType;
		Component m_TargetComponent;

		FieldInfo m_TargetField;

		CompareType m_CompareType;
		object m_CompareValue;


		public enum CompareType
        {
			Unknown = 0,

			Same,

			Grater,
			Less,

			SameAndGrater,
			SameAndLess,
		}

		public override void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
		{
			base.Setup(xmlAttributes, baseTree);

			if (xmlAttributes["Type"] != null &&
				string.IsNullOrEmpty(xmlAttributes["Type"].Value) == false)
			{
				m_TargetType = Type.GetType(xmlAttributes["Type"].Value);
				if (m_TargetType != null) m_TargetComponent = baseTree.GetComponent(m_TargetType);
			}

			var memberName = xmlAttributes["Field"] != null ? xmlAttributes["GetField"].Value : string.Empty;
			if (m_TargetType != null &&
				string.IsNullOrEmpty(memberName) == false)
			{
				m_TargetField = m_TargetType.GetField(memberName);
			}

			m_CompareType = CompareType.Unknown;
			if (xmlAttributes["CompareType"] != null &&
				string.IsNullOrEmpty(xmlAttributes["CompareType"].Value) == false)
			{
				CompareType result;
				if (Enum.TryParse<CompareType>(xmlAttributes["CompareType"].Value, out result))
				{
					m_CompareType = result;
				}
				else
				{
					switch (xmlAttributes["CompareType"].Value)
					{
						case "==":
							m_CompareType = CompareType.Same;
							break;
						case ">":
							m_CompareType = CompareType.Grater;
							break;
						case "<":
							m_CompareType = CompareType.Less;
							break;
						case ">=":
							m_CompareType = CompareType.SameAndGrater;
							break;
						case "<=":
							m_CompareType = CompareType.SameAndLess;
							break;
					}
				}
			}

			if (m_TargetField != null &&
				xmlAttributes["CompareValue"] != null &&
				string.IsNullOrEmpty(xmlAttributes["CompareValue"].Value) == false)
			{
				var parseMethod = m_TargetField.FieldType.GetMethod("Parse");
				m_CompareValue = parseMethod.Invoke(m_TargetField.FieldType, new object[] { xmlAttributes["CompareValue"].Value });
			}
		}

		public override bool CheckCondition()
        {
			if (m_TargetType == null) return false;
			if (m_TargetField == null) return false;
			if (m_CompareValue == null) return false;

			int compareResult = 0;
			var fieldValue = m_TargetField.GetValue(m_TargetComponent);
			if (fieldValue != null && 
				fieldValue is IComparable)
            {
				compareResult = ((IComparable)fieldValue).CompareTo(m_CompareValue);
			}

			switch (m_CompareType)
            {
				case CompareType.Same: return compareResult == 0;
				case CompareType.Grater: return compareResult > 0;
				case CompareType.Less: return compareResult < 0;
				case CompareType.SameAndGrater: return compareResult >= 0;
				case CompareType.SameAndLess: return compareResult <= 0;
			}

			return false;
        }
    }
}
