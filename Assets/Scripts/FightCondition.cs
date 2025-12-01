using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FightCondition", menuName = "Condition/FightCondition")]
    public class FightCondition: Condition
    {
        [SerializeField]private LevelData levelData;
        
        [Tooltip("if false check if is loose, else check if isw win")]
        [SerializeField] private bool isForWin;
        public override bool isComplete(PlayerConditionManager manager)
        {
            return  manager.CheckLevel(levelData,isForWin);
        }
    }
