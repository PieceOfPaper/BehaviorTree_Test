using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeCondition_ComponentField : NodeConditionBase
	{
		[NodeAttribute("Type", NodeAttributeOptionType.Required)] string m_Type;
		[NodeAttribute("Field", NodeAttributeOptionType.Required)] string m_Field;
		[NodeAttribute("CompareType", NodeAttributeOptionType.Required)] string m_CompareType;
		[NodeAttribute("CompareValue", NodeAttributeOptionType.Required)] string m_CompareValue;

		Type m_TargetType;
		Component m_TargetComponent;

		FieldInfo m_TargetField;
		PropertyInfo m_TargetProperty;
		MethodInfo m_TargetMethod;

		CompareType m_ParsedCompareType;
		object m_ParsedCompareValue;


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

			if (string.IsNullOrEmpty(m_Type) == false)
			{
				m_TargetType = Type.GetType(m_Type);
				if (m_TargetType != null) m_TargetComponent = baseTree == null ? null : baseTree.GetComponent(m_TargetType);
			}

			if (string.IsNullOrEmpty(m_Field) == false)
			{
				m_TargetField = m_TargetType.GetField(m_Field);
				m_TargetProperty = m_TargetType.GetProperty(m_Field);
				m_TargetMethod = m_TargetType.GetMethod(m_Field);
			}

			if (string.IsNullOrEmpty(m_CompareType) == false)
			{
				CompareType result;
				if (Enum.TryParse<CompareType>(m_CompareType, out result))
				{
					m_ParsedCompareType = result;
				}
				else
				{
					switch (xmlAttributes["CompareType"].Value)
					{
						case "==":
							m_ParsedCompareType = CompareType.Same;
							break;
						case ">":
							m_ParsedCompareType = CompareType.Grater;
							break;
						case "<":
							m_ParsedCompareType = CompareType.Less;
							break;
						case ">=":
							m_ParsedCompareType = CompareType.SameAndGrater;
							break;
						case "<=":
							m_ParsedCompareType = CompareType.SameAndLess;
							break;
					}
				}
			}

			if (string.IsNullOrEmpty(m_CompareValue) == false)
			{
				Type fieldType = null;

				if (m_TargetField != null)
					fieldType = m_TargetField.FieldType;
				else if (m_TargetProperty != null)
					fieldType = m_TargetProperty.PropertyType;
				else if (m_TargetMethod != null && m_TargetMethod.ReturnType != null && m_TargetMethod.ReturnType != typeof(void))
					fieldType = m_TargetMethod.ReturnType;

				m_ParsedCompareValue = Util.Parse(fieldType, m_CompareValue);
			}
		}

		public override bool CheckCondition()
        {
			if (m_TargetType == null) return false;
			if (m_ParsedCompareValue == null) return false;

			int compareResult = 0;

            if (m_TargetField != null)
            {
                var fieldValue = m_TargetField.GetValue(m_TargetComponent);
                if (fieldValue != null &&
                    fieldValue is IComparable)
                {
                    compareResult = ((IComparable)fieldValue).CompareTo(m_ParsedCompareValue);
                }
            }
			else if (m_TargetProperty != null)
			{
				var propertyValue = m_TargetProperty.GetValue(m_TargetComponent);
				if (propertyValue != null &&
					propertyValue is IComparable)
				{
					compareResult = ((IComparable)propertyValue).CompareTo(m_ParsedCompareValue);
				}
			}
			else if (m_TargetMethod != null)
			{
				var methodValue = m_TargetMethod.Invoke(m_TargetComponent, null);
				if (methodValue != null &&
					methodValue is IComparable)
				{
					compareResult = ((IComparable)methodValue).CompareTo(m_ParsedCompareValue);
				}
			}

			switch (m_ParsedCompareType)
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
