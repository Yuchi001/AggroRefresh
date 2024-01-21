using System;
using System.Collections.Generic;
using System.Linq;
using ItemPack;
using ItemPack.Enum;
using ItemPack.ScriptableObjects;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using UnityEngine.UIElements;

namespace Managers
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private List<SlotUI> slots;

        private void Awake()
        {
            PlayerOngoingStats.OnPickItemToEq += PickItemToEqToEq;
        }

        private void OnDisable()
        {
            PlayerOngoingStats.OnPickItemToEq -= PickItemToEqToEq;
        }

        private void PickItemToEqToEq(SoEqItem item, int slotId)
        {
            var availableSlots = slots.Where(s => s.slotType == item.EquipmentItemType) as List<SlotUI>;
            if (availableSlots == null || availableSlots.Count <= slotId)
            {
                Debug.LogError("Check if slots are setup correctly");
                return;
            }
            
            availableSlots[slotId].EquipItem(item);
        }
    }

    [System.Serializable]
    public class SlotUI
    {
        public Sprite baseSlotImage;
        public Image slotImage;
        public EEquipmentItemType slotType;
        public SoEqItem equippedItem;

        public void EquipItem<T>(T item) where T: SoEqItem
        {
            equippedItem = item;
            slotImage.sprite = item.ItemSprite;
        }

        public void UnEquipItem()
        {
            equippedItem = null;
            slotImage.sprite = baseSlotImage;
        }
    }
}