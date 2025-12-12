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

        public override string GetString()
        {
            return "Dialogue/" + SpeakerName + "/" + DialogueLine;
        }
    }
}