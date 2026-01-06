namespace Nodes
{
	public class BaseNode : XNode.Node 
	{
		// c'était obligatoire le string ? tu pouvais pas passer par un enum ? j'avoue ne pas avoir assez utilisé Xnode,
		// ignore si tu peux pas
		public virtual string GetString()
		{
			return null;
		}
	}
}