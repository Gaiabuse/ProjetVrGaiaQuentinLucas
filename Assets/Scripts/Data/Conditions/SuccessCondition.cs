using Exploration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Conditions
{
    [CreateAssetMenu(fileName = "SuccessCondition", menuName = "Condition/SuccessCondition")]
    public class SuccessCondition : Condition
    {
        [FormerlySerializedAs("successData")] [SerializeField] private ObjectData objectData;
        public override bool IsComplete(PlayerManager manager)
        {
            return manager.CheckObjectObtained(objectData);
        }
    }
}
