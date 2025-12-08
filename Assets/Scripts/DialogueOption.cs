using System;
using UnityEngine;

[Serializable]
public class DialogueOption 
{
    public DialogueGraph dialogue;
    //[Tooltip("if true, it's default dialogue with no conditions")]
    [SerializeField]public bool isFallback = false;
    //[Tooltip("all conditions need true for the dialogue conditions are true")]
    public Condition[] conditions;
}

