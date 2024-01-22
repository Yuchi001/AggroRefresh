using System;
using System.Collections.Generic;
using Enum;
using ItemPack.EnchantmentPack;
using ItemPack.Enum;
using ItemPack.ScriptableObjects;
using SoulPack.Enum;
using UnityEngine;
using WeaponPack.ScriptableObjects;

namespace PlayerPack.PlayerOngoingStatsPack
{
    public class PlayerOngoingStats : MonoBehaviour
    {
        #region singleton

        public static PlayerOngoingStats Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        private ESoulType? _pickedSoul = null;

        private List<ItemSlot<SoWeapon>> _weaponSlots = new()
        {
            null,
            null,
        };
        private List<ItemSlot<SoSoulRing>> _ringSlots = new()
        {
            null,
            null,
            null,
        };
        //todo: chest slot
        //todo: boots slot

        public delegate void PickItemDelegate(SoEqItem item, int slotId);
        public static event PickItemDelegate OnPickItemToEq;

        public delegate void PickEnchantmentDelegate(SoEnchantment enchantment);
        public static event PickEnchantmentDelegate OnPickEnchantment;

        public void PickSoul(ESoulType soulType)
        {
            _pickedSoul = soulType;
            //todo: pick soul mechanics animations and some shit like that idk
        }

        public void PickUpItem(SoItem item)
        {
            Debug.Log(item.GetItemName());
            
            foreach (var stat in item.Stats)
            {
                PlayerBase.PlayerBase.Instance.Stats.ModifyStat(stat.statType, stat.statValue, item.ItemType == EItemType.Equipment);
            }
            
            if (item.ItemType is EItemType.Enchantment)
            {
                var enchantment = item as SoEnchantment;
                if (enchantment == null) return;
                enchantment.PickUp();
                OnPickEnchantment?.Invoke(enchantment);
                return;
            }
            
            var eqItem = item as SoEqItem;
            if (eqItem == null)
            {
                Debug.LogError("Item should be of type SoEqItem but it wasn't!");
                return;
            }
            switch (eqItem.EquipmentItemType)
            {
                case EEquipmentItemType.Weapon:
                    var weapon = eqItem as SoWeapon;
                    for (var i = 0; i < _weaponSlots.Count; i++)
                    {
                        if (_weaponSlots[i] is not null) continue;
                        
                        _weaponSlots[i] = new ItemSlot<SoWeapon>(weapon);
                        OnPickItemToEq?.Invoke(weapon, i);
                        return;
                    }
                    PlaceItemInInventory(weapon);
                    return;
                case EEquipmentItemType.Ring:
                    var ring = eqItem as SoSoulRing;
                    for (var i = 0; i < _ringSlots.Count; i++)
                    {
                        if(_ringSlots[i] is not null) continue;
                        
                        _ringSlots[i] = new ItemSlot<SoSoulRing>(ring);
                        OnPickItemToEq?.Invoke(ring, i);
                        return;
                    }
                    PlaceItemInInventory(ring);
                    return;
                case EEquipmentItemType.Boots:
                    // todo: pickup boots
                    break;
                case EEquipmentItemType.Armor:
                    // todo: pickup armor
                    break;
                default:
                    Debug.LogError($"Item of type {eqItem.EquipmentItemType} does not have pick up logic!");
                    return;
            }
        }

        private void PlaceItemInInventory(SoEqItem item)
        {
            
        }
        
        private void DropItem(SoEqItem item)
        {
            //todo: droping item mechanic
        }

        public float GetStatValue(EStatType statType)
        {
            return PlayerBase.PlayerBase.Instance.Stats.GetStatValue(statType);
        }
    }

    public class ItemSlot<T> where T: SoEqItem
    {
        public T Item { get; private set; }
        
        public ItemSlot(T item)
        {
            Item = item;
        }
    }
}

