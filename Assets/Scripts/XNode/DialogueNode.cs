using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : BaseNode
{
    [Input] public string Entry;
    [Output(dynamicPortList = true)] public string[] Choices;

    public string SpeakerName;
    [TextArea(4,Int32.MaxValue)]public string DialogueLine;

    public override string GetString()
    {
        return "DialogueNode/" + SpeakerName + "/" + DialogueLine;
    }
}