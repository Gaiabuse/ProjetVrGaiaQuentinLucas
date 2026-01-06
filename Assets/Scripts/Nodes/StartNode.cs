using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nodes
{
	public class StartNode : BaseNode 
	{
		[Output] public string Exit;

		public bool SetPosition = true;
		public List<CharacterStartSettings> Characters = new ();
		public Vector3 StartDialoguePosition;
		public override string GetString()
		{
			return "Start";
		}
	}
}

[System.Serializable]
public class CharacterStartSettings
{
	public string CharacterName;
	public bool IsPresent = true;
	public Vector3 Position;
	public Quaternion Rotation;
}
