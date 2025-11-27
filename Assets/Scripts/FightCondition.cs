using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class FightCondition: Condition
    {
        [SerializeField]private LevelData levelData;
        
        [Tooltip("if false check if is loose, else check if isw win")]
        [SerializeField] private bool isForWin;
        public override bool isComplete(PlayerConditionManager manager)
        {
            return  manager.CheckLevel(levelData,isForWin);
            return  manager.CheckLevel(levelData,isForWin);
        }
    }
}