using Exploration;
using UnityEngine;

namespace Data.Conditions
{
    [CreateAssetMenu(fileName = "SuccessCondition", menuName = "Condition/SuccessCondition")]
    public class SuccessCondition : Condition
    {
        [SerializeField] private SuccessData successData;
        public override bool IsComplete(PlayerManager manager)
        {
            return manager.CheckSuccessObtained(successData);
        }
    }
}
