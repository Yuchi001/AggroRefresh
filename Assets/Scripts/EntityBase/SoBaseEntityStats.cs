using System;
using System.Collections.Generic;
using System.Linq;
using Enum;
using UnityEngine;

namespace SoulPack
{
    [CreateAssetMenu(fileName = "new EntityStats", menuName = "Custom/EntityStats", order = 0)]
    public class SoBaseEntityStats : ScriptableObject
    {
        [SerializeField] private List<Stat> statList = new();

        public List<Stat> StatList => statList;

        public float GetBaseStatVal(EStatType statType)
        {
            var stat = statList.FirstOrDefault(s => s.statType == statType);
            if (stat == default) return -1;
            return stat.statValue;
        }
    }

    [Serializable]
    public class Stat
    {
        public EStatType statType;
        public float statValue; 
    }
}