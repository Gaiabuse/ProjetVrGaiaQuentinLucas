using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class EndNode : BaseNode 
{
	[Input] public string Entry;

	public override string GetString()
	{
		return "End";
	}
}