using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SuccessCondition", menuName = "Condition/SuccessCondition")]
public class SuccessCondition : Condition
{
    [SerializeField] private SuccessData successData;
    public override bool IsComplete(PlayerManager manager)
    {
        return manager.CheckSuccessObtained(successData);
    }
}
