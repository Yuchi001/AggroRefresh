using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enum;
using ItemPack.Enum;
using ItemPack.ScriptableObjects;
using PlayerPack.PlayerOngoingStatsPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private List<SlotUI> slots;
        [SerializeField] private List<StatUI> stats;

        private static PlayerOngoingStats PlayerStats => PlayerOngoingStats.Instance;
        private void Awake()
        {
            PlayerOngoingStats.OnPickItemToEq += PickItemToEq;
            PlayerOngoingStats.OnPickEnchantment += UpdateSelectedFields;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerStats != null);
            
            foreach (var stat in stats)
            {
                stat.statTextField.text = ": " + Mathf.CeilToInt(PlayerStats.GetStatValue(stat.statType));
            }
        }

        private void OnDisable()
        {
            PlayerOngoingStats.OnPickItemToEq -= PickItemToEq;
            PlayerOngoingStats.OnPickEnchantment -= UpdateSelectedFields;
        }
        
        private void UpdateSelectedFields(SoItem item)
        {
            foreach (var stat in item.Stats)
            {
                var statUI = stats.FirstOrDefault(s => s.statType == stat.statType);
                if(statUI == default) continue;

                statUI.statTextField.text = ": " + Mathf.CeilToInt(PlayerStats.GetStatValue(stat.statType));
            }
        }

        private void PickItemToEq(SoEqItem item, int slotId)
        {
            UpdateSelectedFields(item);
            
            var availableSlots = new List<SlotUI>();
            foreach (var slot in slots)
            {
                if(slot.slotType != item.EquipmentItemType) continue;
                availableSlots.Add(slot);
            }
            if (availableSlots.Count <= slotId)
            {
                Debug.LogError("Check if slots are setup correctly");
                return;
            }
            
            availableSlots[slotId].EquipItem(item);
        }
    }

    [System.Serializable]
    public class StatUI
    {
        public TextMeshProUGUI statTextField;
        public EStatType statType;
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