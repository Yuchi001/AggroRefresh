using System.Collections.Generic;
using ItemPack.Enum;
using SoulPack;
using UnityEngine;

namespace ItemPack.ScriptableObjects
{
    public abstract class SoEqItem : SoItem
    {
        [SerializeField] protected EEquipmentItemType equipmentItemType;
        public EEquipmentItemType EquipmentItemType => equipmentItemType;
    }
}