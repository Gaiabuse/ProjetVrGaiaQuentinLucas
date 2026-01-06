using System;
using UnityEngine;

namespace Nodes
{
    public class DialogueNode : BaseNode
    {
        [Input] public string Entry;
        [Output(dynamicPortList = true)] public string[] Choices;

        public string SpeakerName;
        [TextArea(4,Int32.MaxValue)]public string DialogueLine;

        public string AnimationTrigger;
        [Tooltip("one voice lines by | in the dialogue line")]
        public AudioClip[] Voices;
        public override string GetString()
        {
            // hésite pas à interpoler pour éviter des allocs inutiles potentielles
            return "Dialogue/" + SpeakerName + "/" + DialogueLine;
        }
    }
}