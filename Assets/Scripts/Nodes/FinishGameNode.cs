namespace Nodes
{
    public class FinishGameNode : BaseNode 
    {
        [Input] public string Entry;

        public override string GetString()
        {
            return "Finish";
        }
    }
}