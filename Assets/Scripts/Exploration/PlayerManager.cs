using System.Collections.Generic;
using Data;
using Nodes;
using UnityEngine;

namespace Exploration
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager INSTANCE;
        [SerializeField] private Transform player;
        [SerializeField] private GameObject locomotion;
        private readonly List<LevelData> _levelDatasWins = new();
        private readonly List<LevelData> _levelDatasLose = new();
        private readonly List<ObjectData> _objectObtained = new();
        private readonly List<DialogueNode> _dialogueSeen = new();
        private void Awake()
        {
            INSTANCE = this;
            Debug.Log(INSTANCE);
        }

        private void OnEnable()
        {
            GameManager.ReturnToMainMenu += ClearList;
        }

        private void OnDisable()
        {
            GameManager.ReturnToMainMenu -= ClearList;
        }
        private void ClearList()
        {
            _levelDatasWins.Clear();
            _levelDatasLose.Clear();
            _objectObtained.Clear();
            _dialogueSeen.Clear();
        }
        public void SetCanMove(bool canMove)
        {
            locomotion.SetActive(canMove);
        }

        public void TeleportPlayer(Vector3 position)
        {
            player.position = position;
        }

        public Vector3 GetPlayerPosition()
        {
            return player.position;
        }
        public void AddLevelData(LevelData levelData,bool isWin)
        {
            if (isWin)
            {
                _levelDatasWins.Add(levelData); 
            }
            else
            {
                _levelDatasLose.Add(levelData);
            }
        
        }
        public void AddObject(ObjectData data)
        {
            _objectObtained.Add(data);
        }

        public bool CheckObjectObtained(ObjectData data)
        {
            return _objectObtained.Contains(data);
        }

        public void AddDialogueNode(DialogueNode dialogueNode)
        {
            _dialogueSeen.Add(dialogueNode);
        }

        public bool CheckLevelIsWin(LevelData levelData, bool isWin)
        {
            if (!_levelDatasWins.Contains(levelData) && !_levelDatasLose.Contains(levelData))
            {
                return true;// if you don't do the level, so you win the level
            }
            return isWin ? _levelDatasWins.Contains(levelData) : _levelDatasLose.Contains(levelData);
        }

        public bool CheckDialogueSeen(DialogueNode dialogueNode)
        {
            return _dialogueSeen.Contains(dialogueNode);
        }

    }
}
