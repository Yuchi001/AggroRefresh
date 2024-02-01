using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enum;
using ItemPack.ScriptableObjects;
using PlayerPack.PlayerBase;
using PlayerPack.PlayerOngoingStatsPack;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public sealed class InventoryUiHandler : MonoBehaviour
    {
        [SerializeField] private GameObject dragHandlerPrefab;
        [SerializeField] private List<Utils.Tuple<EStatType, TextMeshProUGUI>> statDisplays;

        private List<SlotUI> uiSlots = new();
        private Animator InvAnim => GetComponent<Animator>();
        private bool opened = false;

        private void Awake()
        {
            PlayerOngoingStats.OnEquipItem += OnEquipItem;
            PlayerOngoingStats.OnUpdatePlayerStats += OnUpdatePlayerStats;
            foreach (var slot in transform.GetComponentsInChildren<SlotUI>())
            {
                uiSlots.Add(slot);
                slot.Setup(dragHandlerPrefab);
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerBase.Instance != null);
            OnUpdatePlayerStats();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            opened = !opened;
            InvAnim.SetTrigger(opened ? "enter" : "exit");
        }

        private void OnDisable()
        {
            PlayerOngoingStats.OnEquipItem -= OnEquipItem;
            PlayerOngoingStats.OnUpdatePlayerStats -= OnUpdatePlayerStats;
        }

        private void OnUpdatePlayerStats()
        {
            var playerStats = PlayerBase.Instance.Stats;
            foreach (var statDisplay in statDisplays)
            {
                statDisplay.value.text = ": " + Mathf.CeilToInt(playerStats.GetStatValue(statDisplay.key)).ToString();
            }
        }

        private void OnEquipItem(SoEqItem item, int id)
        {
            var slot = uiSlots.FirstOrDefault(s => s.GetId() == id);
            if (slot == default)
            {
                Debug.LogError("UI slots are not setup correctly.");
                return;
            }

            if (item == null) slot.RemoveItem();
            else slot.EquipItem(item);
        }
    }
}