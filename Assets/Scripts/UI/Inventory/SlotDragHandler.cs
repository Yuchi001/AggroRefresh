using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Managers;
using PlayerPack.PlayerOngoingStatsPack;
using UI.Inventory.Enum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI.Inventory
{
    public class SlotDragHandler : MonoBehaviour
    {
        [SerializeField] private Image slotItemImage;
        [SerializeField] private LayerMask slotLayer;
        private SlotUI _slot;
        private static PlayerOngoingStats PlayerStats => PlayerOngoingStats.Instance;

        public delegate void ItemEndDragDelegate(SlotUI slotUI, SlotUI hitSlotUI);
        public static event ItemEndDragDelegate OnItemEndDrag;
        
        private RectTransform MainWindow => GetComponent<RectTransform>();

        [CanBeNull] private SlotUI _currentOverlap = null;

        public void Setup(SlotUI slot)
        {
            _slot = slot;
            slotItemImage.sprite = slot.GetItem().ItemSprite;
        }
        
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
        
        private void Update()
        {
            MainWindow.anchoredPosition = UiManager.Instance.MainCanvasRect.anchoredPosition - (Vector2)Input.mousePosition / UiManager.Instance.MainCanvas.scaleFactor;
            MainWindow.anchoredPosition *= -1;
            MainWindow.transform.SetAsLastSibling();
        }

        public void OnEndDrag(BaseEventData data)
        {
            var pointerEventData = (PointerEventData)data;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count == 1 && raycastResults[0].sortingLayer == 0)
            {
                PlayerOngoingStats.Instance.DropItem(_slot.GetId());
                Destroy(gameObject);
                return;
            }

            foreach (var res in raycastResults)
            {
                if(!res.gameObject.TryGetComponent(out SlotUI hitSlot)) continue;
                
                OnItemEndDrag?.Invoke(_slot, hitSlot);
                PlayerOngoingStats.Instance.MoveItem(_slot.GetId(), hitSlot.GetId());
                break;
            }
            
            Destroy(gameObject);
        }
    }
}