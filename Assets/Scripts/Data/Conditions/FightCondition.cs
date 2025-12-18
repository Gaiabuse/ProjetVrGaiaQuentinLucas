using Exploration;
using UnityEngine;

namespace Data.Conditions
{
    [CreateAssetMenu(fileName = "FightCondition", menuName = "Condition/FightCondition")]
    public class FightCondition: Condition
    {
        [SerializeField]private LevelData levelData;
        [Tooltip("if false check if is loose, else check if is win")]
        [SerializeField] private bool isForWin;
        public override bool IsComplete(PlayerManager manager)
        {
            if (manager == null)
            {
                Debug.LogError("PlayerManager est NULL");
                return false;
            }
            if (levelData == null)
            {
                Debug.LogError("levelData est NULL (pas assigné dans l'asset FightCondition)");
                return false;
            }
            return  manager.CheckLevelIsWin(levelData,isForWin);
        }
    }
}
