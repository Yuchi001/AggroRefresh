using System;
using ItemPack.Enum;
using ItemPack.ScriptableObjects;
using Managers;
using PlayerPack.PlayerOngoingStatsPack;
using UI.Inventory.Enum;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Inventory
{
    [System.Serializable]
    public class SlotUI : MonoBehaviour
    {
        [SerializeField] protected Image slotImage;
        [SerializeField] protected Sprite selectedBackground;
        [SerializeField] protected int id;
        protected ItemSlot ItemSlot = null;
        protected GameObject DragPrefab;
        private Sprite _defaultFrameImg;
        private Image _frameImage;
        protected SlotDragHandler SpawnedDrag;

        public virtual void EquipItem(SoEqItem item)
        {
            ItemSlot = new ItemSlot(item);
            slotImage.sprite = item.ItemSprite;
        }

        public virtual void RemoveItem()
        {
            ItemSlot = null;
            slotImage.sprite = null;
        }

        protected virtual void Update()
        {
            if (IsEmpty()) return;

            slotImage.sprite = SpawnedDrag ? null : ItemSlot.Item.ItemSprite;
        }

        public void Setup(GameObject dragPrefab)
        {
            _frameImage = GetComponent<Image>();
            _defaultFrameImg = _frameImage.sprite;
            DragPrefab = dragPrefab;

            var eventTrigger = gameObject.AddComponent<EventTrigger>();
            var pointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnter.callback.AddListener((data) => OnHover());
            var pointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExit.callback.AddListener((data) => OnEndHover());
            var beginDrag = new EventTrigger.Entry
            {
                eventID = EventTriggerType.BeginDrag
            };
            beginDrag.callback.AddListener((data) => OnBeginDrag());
            var endDrag = new EventTrigger.Entry
            {
                eventID = EventTriggerType.EndDrag
            };
            endDrag.callback.AddListener(OnEndDrag);
            
            eventTrigger.triggers.Add(pointerEnter);
            eventTrigger.triggers.Add(pointerExit);
            eventTrigger.triggers.Add(beginDrag);
            eventTrigger.triggers.Add(endDrag);
        }

        public void OnBeginDrag()
        {
            if (IsEmpty()) return;
            
            var dragScript = Instantiate(
                    DragPrefab, 
                    transform.position, 
                    Quaternion.identity,
                    UiManager.Instance.MainCanvasTransform)
                .GetComponent<SlotDragHandler>();
            dragScript.Setup(this);
            SpawnedDrag = dragScript;
        }

        public void OnEndDrag(BaseEventData data)
        {
            if (SpawnedDrag == null) return;
            SpawnedDrag.OnEndDrag(data);
        }

        public void OnHover()
        {
            _frameImage.sprite = selectedBackground;
        }

        public void OnEndHover()
        {
            _frameImage.sprite = _defaultFrameImg;
        }
        
        public bool IsEmpty()
        {
            return ItemSlot == null;
        }

        public SoEqItem GetItem()
        {
            return ItemSlot?.Item;
        }

        public int GetId()
        {
            return id;
        }
    }
}