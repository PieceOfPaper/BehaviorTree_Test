using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class NodeAction : NodeBase
	{
		public enum ActionType
		{
			NoAction = 0,
			SendMessage,
			BrodcastMessage,
			SendMessageUpwards,
			Max,
		}

		protected ActionType _actionType;
		public ActionType actionType
		{
			protected set
			{
				_actionType = value;
			}
			get
			{
				return _actionType;
			}
		}


		protected string className = string.Empty;
		protected string methodName = string.Empty;
		protected object sendValue = null;
		
		public NodeAction(System.Xml.XmlAttributeCollection xmlAttributes, BehaviorTree baseTree) : base(xmlAttributes, baseTree)
		{
			this.actionType = ActionType.NoAction;
			if (xmlAttributes["Type"] != null)
			{
				string strType = xmlAttributes["Type"].Value.ToLower();
				for (int i = 0; i < (int)ActionType.Max; i ++)
				{
					if (((ActionType)i).ToString().ToLower().Contains(strType))
					{
						this.actionType = (ActionType)i;
						break;
					}
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
					break;
				case ActionType.BrodcastMessage:
					baseTree.BroadcastMessage(methodName, sendValue);
					break;
				case ActionType.SendMessageUpwards:
					baseTree.SendMessageUpwards(methodName, sendValue);
					break;
				default:
					nodeState = NodeState.Success;
					break;
			}

			yield return null;
		}
	}
}
