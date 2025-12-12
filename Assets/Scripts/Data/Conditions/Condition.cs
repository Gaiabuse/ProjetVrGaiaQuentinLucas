using System;
using Exploration;
using UnityEngine;

namespace Data.Conditions
{
    [Serializable]
    public abstract class Condition: ScriptableObject
    {
        public abstract bool IsComplete(PlayerManager manager);
    }
}
