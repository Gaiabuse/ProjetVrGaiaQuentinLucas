using System;
using UnityEngine;

[Serializable]
public class ObjectCondition : Condition
{
    [SerializeField] private ObjectData objectData;
    public override bool isComplete(PlayerConditionManager manager)
    {
        return manager.checkObjectObtained(objectData);
    }
}
