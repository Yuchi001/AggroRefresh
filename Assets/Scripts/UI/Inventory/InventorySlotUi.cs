using ItemPack.ScriptableObjects;
using UnityEngine;

namespace UI.Inventory
{
    public class InventorySlotUi : SlotUI
    {
        public override void EquipItem(SoEqItem item)
        {
            base.EquipItem(item);
            slotImage.color = Color.white;
        }

        public override void RemoveItem()
        {
            base.RemoveItem();
            slotImage.color = Color.clear;
        }

        protected override void Update()
        {
            if (IsEmpty()) return;
            
            base.Update();
            
            slotImage.color = SpawnedDrag ? Color.clear : Color.white;
        }
    }
}