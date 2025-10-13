using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class ConditionData 
{
    #region Enum

    public enum ConditionType
    {
        anxiety,
        obj
    }

    #endregion
    [LabelWidth(70)]
    public ConditionType Type;

    [ShowIf(nameof(IsLife))] public int lifeParameter;

    [ShowIf(nameof(IsObject))] public string ObjectId;

    private bool IsLife() => Type == ConditionType.anxiety;
    private bool IsObject() => Type == ConditionType.obj;
}
