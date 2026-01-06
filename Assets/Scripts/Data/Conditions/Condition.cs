using System;
using Exploration;
using UnityEngine;

namespace Data.Conditions
{
    // C'est pas mal ! peut être que tu aurais pu simplifier avec une interface au lieu de l'abstraction dans ton cas,
    // vu que tu n'as besoin que d'un bool, mais ca fait le taf
    [Serializable]
    public abstract class Condition: ScriptableObject
    {
        public abstract bool IsComplete(PlayerManager manager);
    }
}
