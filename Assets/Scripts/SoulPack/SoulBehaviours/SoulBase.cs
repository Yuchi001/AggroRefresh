using System;
using System.Collections.Generic;
using AbilityPack;
using SoulPack.Enum;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoulPack.SoulBehaviours
{
    public class SoulBase : MonoBehaviour
    {
        [SerializeField] protected Image soulImage;
        [SerializeField] protected ESoulType soulType;
        [SerializeField] protected List<AbilityLevelTuple> soulMainAbilities;
        [SerializeField] protected List<AbilityLevelTuple> soulSecondAbilities;
        [SerializeField] protected List<AbilityLevelTuple> soulDashAbilities;
    }
    
    [System.Serializable]
    public class AbilityLevelTuple : Utils.Tuple<int, SoAbilityProps>{}
}