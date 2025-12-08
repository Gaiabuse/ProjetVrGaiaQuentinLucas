using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class PlayerConditionManager : MonoBehaviour
{
    public static PlayerConditionManager instance;


    private List<LevelData> levelDatasWins = new();
    private List<LevelData> levelDatasLose = new();
    private List<SuccessData> objectObtained = new();
    private List<DialogueNode> dialogueSeen = new();
    private void Awake()
    {
        instance = this;
    }

    public void AddLevelData(LevelData levelData,bool isWin)
    {
        if (isWin)
        {
            levelDatasWins.Add(levelData); 
        }
        else
        {
            levelDatasLose.Add(levelData);
        }
        
    }

    public void AddSuccess(SuccessData data)
    {
        objectObtained.Add(data);
    }

    public bool CheckSuccessObtained(SuccessData data)
    {
        return objectObtained.Contains(data);
    }

    public void AddDialogueNode(DialogueNode dialogueNode)
    {
        dialogueSeen.Add(dialogueNode);
    }

    public bool CheckLevel(LevelData levelData, bool isWin)
    {
        if (isWin)
        {
            return levelDatasWins.Contains(levelData);
        }
        return levelDatasLose.Contains(levelData);
    }

    public bool checkDialogueSeen(DialogueNode dialogueNode)
    {
        return dialogueSeen.Contains(dialogueNode);
    }
    
}
