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

		protected ActionType actionType = ActionType.NoAction;
		protected string className = string.Empty;
		protected string methodName = string.Empty;
		protected object sendValue = null;

		public override void Setup(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree)
		{
			base.Setup(xmlAttributes, baseTree);

			this.actionType = ActionType.NoAction;
			if (xmlAttributes["Type"] != null &&
				string.IsNullOrEmpty(xmlAttributes["Type"].Value) == false)
			{
				ActionType result;
				if (System.Enum.TryParse<ActionType>(xmlAttributes["Type"].Value, out result))
				{
					this.actionType = result;
				}
			}

			this.methodName = xmlAttributes["MethodName"] != null ? xmlAttributes["MethodName"].Value : string.Empty;

			if (xmlAttributes["SendValue"] != null)
			{
				int ivalue;
				float fvalue;
				if (int.TryParse(xmlAttributes["SendValue"].Value, out ivalue))
				{
					sendValue = ivalue;
				}
				else if (float.TryParse(xmlAttributes["SendValue"].Value, out fvalue))
				{
					sendValue = fvalue;
				}

				sendValue = xmlAttributes["SendValue"].Value;
			}
		}

		public override IEnumerator RunningRoutine()
		{
			nodeState = NodeState.Running;

			switch(actionType)
			{
				case ActionType.SendMessage:
					baseTree.SendMessage(methodName, sendValue);
					nodeState = NodeState.Success;
					break;
				case ActionType.BrodcastMessage:
					baseTree.BroadcastMessage(methodName, sendValue);
					nodeState = NodeState.Success;
					break;
				case ActionType.SendMessageUpwards:
					baseTree.SendMessageUpwards(methodName, sendValue);
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
