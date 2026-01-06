using Data;
using UnityEngine.Serialization;

namespace Nodes
{
    public class FightNode : BaseNode
    {
        [Input] public string Entry;
        [Output] public string AsWin;
        [Output] public string AsLoose;

        public bool isTuto;
        public LevelData Level;

        public override string GetString()
        {
            return "Fight";
        }
    }
}
