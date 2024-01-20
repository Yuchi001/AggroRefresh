using ItemPack.Enum;
using UnityEngine;
using Utils;

namespace ItemPack.ScriptableObjects
{
    public abstract class SoItem : ScriptableObject
    {
        [SerializeField] protected Sprite itemSprite;
        [SerializeField] protected string itemName;
        [SerializeField] protected EItemTier itemTier;
        [SerializeField] protected EItemType itemType;

        public EItemType ItemType => itemType;
        public Sprite ItemSprite => itemSprite;
        
        public abstract string GetItemName();
    }
}