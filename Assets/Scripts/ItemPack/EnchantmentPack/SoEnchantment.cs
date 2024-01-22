using ItemPack.ScriptableObjects;
using UnityEngine;

namespace ItemPack.EnchantmentPack
{
    [CreateAssetMenu(fileName = "new Basic Enchantment", menuName = "Custom/Enchantment/StatEnchantment")]
    public class SoEnchantment : SoItem
    {
        public override string GetItemName()
        {
            // todo: Enchantment naming
            return "";
        }

        public virtual void PickUp()
        {
            
        }
    }
}