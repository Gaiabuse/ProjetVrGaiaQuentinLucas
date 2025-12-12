
using System;
using UnityEngine;

[Serializable]
public abstract class Condition: ScriptableObject
{
    public abstract bool IsComplete(PlayerManager manager);
}
