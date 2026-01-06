

using UnityEngine;

namespace Nodes
{
    public class AnimationNode : BaseNode
    {
        [Input] public string Entry;
        [Output] public string Exit;
            
        public string CharacterName;
        public string AnimationTrigger;
        public override string GetString()
        {
            // hésite pas à interpoler pour éviter des allocs inutiles potentielles
            return "Animation";
        }
    }
}
