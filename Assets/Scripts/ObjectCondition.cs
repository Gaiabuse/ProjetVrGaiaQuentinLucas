using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectCondition", menuName = "Condition/ObjectCondition")]
public class ObjectCondition : Condition
{
    [SerializeField] private SuccessData successData;
    public override bool isComplete(PlayerConditionManager manager)
    {
        return manager.CheckSuccessObtained(successData);
    }
}
