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
		PropertyInfo m_TargetProperty;
		MethodInfo m_TargetMethod;

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

			var fieldName = xmlAttributes["Field"] != null ? xmlAttributes["Field"].Value : string.Empty;
			if (m_TargetType != null &&
				string.IsNullOrEmpty(fieldName) == false)
			{
				m_TargetField = m_TargetType.GetField(fieldName);
				m_TargetProperty = m_TargetType.GetProperty(fieldName);
				m_TargetMethod = m_TargetType.GetMethod(fieldName);
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

			if (xmlAttributes["CompareValue"] != null &&
				string.IsNullOrEmpty(xmlAttributes["CompareValue"].Value) == false)
			{
				Type fieldType = null;
				MethodInfo parseMethod = null;

				if (m_TargetField != null)
					fieldType = m_TargetField.FieldType;
				else if (m_TargetProperty != null)
					fieldType = m_TargetProperty.PropertyType;
				else if (m_TargetMethod != null && m_TargetMethod.ReturnType != null && m_TargetMethod.ReturnType != typeof(void))
					fieldType = m_TargetMethod.ReturnType;

				parseMethod = fieldType == null ? null : fieldType.GetMethod("Parse", new Type[] { typeof(string) });
				m_CompareValue = fieldType == null || parseMethod == null ? null : parseMethod.Invoke(fieldType, new object[] { xmlAttributes["CompareValue"].Value });
			}
		}

		public override bool CheckCondition()
        {
			if (m_TargetType == null) return false;
			if (m_CompareValue == null) return false;

			int compareResult = 0;

            if (m_TargetField != null)
            {
                var fieldValue = m_TargetField.GetValue(m_TargetComponent);
                if (fieldValue != null &&
                    fieldValue is IComparable)
                {
                    compareResult = ((IComparable)fieldValue).CompareTo(m_CompareValue);
                }
            }
			else if (m_TargetProperty != null)
			{
				var propertyValue = m_TargetProperty.GetValue(m_TargetComponent);
				if (propertyValue != null &&
					propertyValue is IComparable)
				{
					compareResult = ((IComparable)propertyValue).CompareTo(m_CompareValue);
				}
			}
			else if (m_TargetMethod != null)
			{
				var methodValue = m_TargetMethod.Invoke(m_TargetComponent, null);
				if (methodValue != null &&
					methodValue is IComparable)
				{
					compareResult = ((IComparable)methodValue).CompareTo(m_CompareValue);
				}
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
