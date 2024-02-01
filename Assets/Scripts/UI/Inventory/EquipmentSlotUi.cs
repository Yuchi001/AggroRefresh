using UnityEngine;

namespace UI.Inventory
{
    public class EquipmentSlotUi : SlotUI
    {
        [SerializeField] private Sprite defaultSprite;

        public override void RemoveItem()
        {
            base.RemoveItem();
            slotImage.sprite = defaultSprite;
        }

        protected override void Update()
        {
            if (IsEmpty()) return;

            slotImage.sprite = SpawnedDrag ? defaultSprite : ItemSlot.Item.ItemSprite;
        }
    }
}