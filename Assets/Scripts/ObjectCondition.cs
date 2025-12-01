using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectCondition", menuName = "Condition/ObjectCondition")]
public class ObjectCondition : Condition
{
    [SerializeField] private ObjectData objectData;
    public override bool isComplete(PlayerConditionManager manager)
    {
        return manager.checkObjectObtained(objectData);
    }
}
