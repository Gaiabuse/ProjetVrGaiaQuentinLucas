using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class PlayerConditionManager : MonoBehaviour
{
    public static PlayerConditionManager instance;


    private List<LevelData> levelDatasWins;
    private List<LevelData> levelDatasLose;
    private List<ObjectData> objectObtained;
    private List<DialogueNode> dialogueSeen;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
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

    public void AddObjectData(ObjectData objectData)
    {
        objectObtained.Add(objectData);
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

    public bool checkObjectObtained(ObjectData objectData)
    {
        return objectObtained.Contains(objectData);
    }
}
