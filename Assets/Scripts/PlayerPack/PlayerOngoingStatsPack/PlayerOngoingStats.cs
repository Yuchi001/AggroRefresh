using System;
using System.Collections.Generic;
using System.Linq;
using Enum;
using ItemPack.EnchantmentPack;
using ItemPack.Enum;
using ItemPack.ScriptableObjects;
using SoulPack.Enum;
using UI.Inventory;
using UI.Inventory.Enum;
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

        [SerializeField] private List<SlotInfo> slots;

        public delegate void EquipItemDelegate(SoEqItem item, int id);
        public static event EquipItemDelegate OnEquipItem;

        public delegate void UpdatePlayerStatsDelegate();
        public static event UpdatePlayerStatsDelegate OnUpdatePlayerStats;

        private ESoulType? _pickedSoul = null;
        
        public void PickSoul(ESoulType soulType)
        {
            _pickedSoul = soulType;
            //todo: pick soul mechanics animations and some shit like that idk
        }

        public bool PickUpItem(SoItem item)
        {
            foreach (var stat in item.Stats)
            {
                PlayerBase.PlayerBase.Instance.Stats.ModifyStat(stat.statType, stat.statValue,
                    item.ItemType == EItemType.Equipment);
            }
            
            OnUpdatePlayerStats?.Invoke();
            
            if (item.ItemType == EItemType.Enchantment)
            {
                var enchantment = item as SoEnchantment;
                if (!enchantment)
                {
                    Debug.LogError("Item was marked as enchantment but it was not of enchantment type");
                    return false;
                }
                ManagePickUpEnchantment(enchantment);
                return false;
            }

            var equipmentItem = item as SoEqItem;
            if (!equipmentItem)
            {
                Debug.LogError("Item was marked as equipment but it was not of equipment type");
                return false;
            }

            var equippedSlotId = -1;

            foreach (var slot in slots.Where(s => s.equipmentItemType == equipmentItem.EquipmentItemType))
            {
                if (slot.ItemSlot != null) continue;
                slot.ItemSlot = new ItemSlot(equipmentItem);
                equippedSlotId = slot.id;
                break;
            }
            
            equippedSlotId = equippedSlotId == -1 ? PlaceItemToInv(equipmentItem) : equippedSlotId;

            if (equippedSlotId == -1) return false;
            
            OnEquipItem?.Invoke(equipmentItem, equippedSlotId);
            return true;
        }

        public int PlaceItemToInv(SoEqItem item)
        {
            foreach (var slot in slots.Where(s => s.equipmentItemType == EEquipmentItemType.None))
            {
                if (slot.ItemSlot != null) continue;

                slot.ItemSlot = new ItemSlot(item);
                return slot.id;
            }

            return -1;
        }

        private void ManagePickUpEnchantment(SoEnchantment enchantment)
        {
            //todo some enchantment will have on pick up effects
        }

        public void MoveItem(int currId, int dirId)
        {
            var currSlot = slots.FirstOrDefault(s => s.id == currId);
            var dirSlot = slots.FirstOrDefault(s => s.id == dirId);
            if (dirSlot == default || currSlot == default)
            {
                Debug.LogError("Slot was not found!");
                return;
            }

            var currItem = currSlot.ItemSlot.Item;
            var dirItem = dirSlot.ItemSlot?.Item;

            var isSameItemType = currItem.EquipmentItemType == dirSlot.equipmentItemType;
            var isSameSlotType = currSlot.equipmentItemType == dirSlot.equipmentItemType;
            var isSecondSlotInvAndCompatible =
                dirSlot.equipmentItemType == EEquipmentItemType.None && (dirSlot.ItemSlot == null || dirItem?.EquipmentItemType == currItem.EquipmentItemType);
            
            if (!isSameItemType && !isSameSlotType && !isSecondSlotInvAndCompatible) return;

            currSlot.ItemSlot = dirItem ? new ItemSlot(dirItem) : null;
            dirSlot.ItemSlot = new ItemSlot(currItem);
            
            OnEquipItem?.Invoke(currSlot.ItemSlot?.Item, currSlot.id);
            OnEquipItem?.Invoke(dirSlot.ItemSlot.Item, dirSlot.id);
        }

        public void DropItem(int slotId)
        {
            var slot = slots.FirstOrDefault(s => s.id == slotId);
            var item = slot?.ItemSlot.Item;
            if (slot == default || item == null)
            {
                Debug.LogError("Check if slots are setup correctly");
                return;
            }
            
            GameManager.Instance.SpawnItem(item, GameManager.Instance.PlayerPosition, 0.5f);
            slot.ItemSlot = null;
            OnEquipItem?.Invoke(null, slotId);
        }
    }

    [System.Serializable]
    public class SlotInfo
    {
        public int id;
        public ItemSlot ItemSlot;
        public EEquipmentItemType equipmentItemType;
    }

    public class ItemSlot
    {
        public SoEqItem Item { get; private set; }

        public ItemSlot(SoEqItem item)
        {
            Item = item;
        }
    }
}

