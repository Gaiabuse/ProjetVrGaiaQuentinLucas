using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace Nodes
{
	[CreateAssetMenu]
	public class DialogueGraph : NodeGraph 
	{ 
		public BaseNode Current;
	}
}