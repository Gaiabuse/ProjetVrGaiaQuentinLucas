namespace Nodes
{
	public class EndNode : BaseNode 
	{
		[Input] public string Entry;

		public override string GetString()
		{
			return "End";
		}
	}
}