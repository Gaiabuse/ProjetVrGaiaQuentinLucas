using System;
using UnityEngine;
[CreateAssetMenu(fileName = "DialogueCondition", menuName = "Condition/DialogueSeenCondition")]
public class DialogueSeenCondition : Condition
{
    [Tooltip("Check in all dialogue graph")]
    [SerializeField] private DialogueNode dialogueNode;
    public override bool IsComplete(PlayerManager manager)
    {
        return manager.CheckDialogueSeen(dialogueNode);
    }
}
