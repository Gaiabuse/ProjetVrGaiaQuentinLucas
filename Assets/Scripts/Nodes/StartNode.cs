using UnityEngine;

namespace Nodes
{
	public class StartNode : BaseNode 
	{
		[Output] public string Exit;

		public bool SetPosition = true;
		public Vector3 StartDialoguePosition;
		public override string GetString()
		{
			return "Start";
		}
	}
}
