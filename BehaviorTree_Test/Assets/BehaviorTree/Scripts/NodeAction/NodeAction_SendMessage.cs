using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeAction_SendMessage : NodeActionBase
	{
		public enum ActionType
		{
			NoAction = 0,
			SendMessage,
			BrodcastMessage,
			SendMessageUpwards,
			Max,
		}

		[NodeAttribute("Type", NodeAttributeOptionType.Required)] ActionType actionType = ActionType.NoAction;
		[NodeAttribute("MethodName", NodeAttributeOptionType.Required)] string methodName;
		[NodeAttribute("SendValue", NodeAttributeOptionType.Optional)] string sendValue;

		object value = null;

		public override void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
		{
			base.Setup(xmlAttributes, baseTree);

            if (sendValue != null)
            {
                int ivalue;
                float fvalue;
                if (int.TryParse(sendValue, out ivalue))
                {
					value = ivalue;
                }
                else if (float.TryParse(sendValue, out fvalue))
                {
					value = fvalue;
                }
				else
				{
					value = sendValue;
				}
            }
        }

		public override IEnumerator RunningRoutine()
		{
			nodeState = NodeState.Running;

			switch(actionType)
			{
				case ActionType.SendMessage:
					if (value == null)
						baseTree.SendMessage(methodName);
					else
						baseTree.SendMessage(methodName, value);
					nodeState = NodeState.Success;
					break;
				case ActionType.BrodcastMessage:
					if (value == null)
						baseTree.BroadcastMessage(methodName);
					else
						baseTree.BroadcastMessage(methodName, value);
					nodeState = NodeState.Success;
					break;
				case ActionType.SendMessageUpwards:
					if (value == null)
						baseTree.SendMessageUpwards(methodName);
					else
						baseTree.SendMessageUpwards(methodName, value);
					nodeState = NodeState.Success;
					break;
				default:
					nodeState = NodeState.Fail;
					break;
			}

			yield return null;
		}
	}
}
