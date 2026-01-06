

using UnityEngine;

namespace Nodes
{
    public class SoundNode : BaseNode
    {
        [Input] public string Entry;
        [Output] public string Exit;
            
        public AudioClip SoundEvent;
        public override string GetString()
        {
            // hésite pas à interpoler pour éviter des allocs inutiles potentielles
            return "Sound";
        }
    }
}
