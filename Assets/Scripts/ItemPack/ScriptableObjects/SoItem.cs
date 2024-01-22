using System.Collections.Generic;
using ItemPack.Enum;
using SoulPack;
using UnityEngine;

namespace ItemPack.ScriptableObjects
{
    public abstract class SoItem : ScriptableObject
    {
        [SerializeField] protected Sprite itemSprite;
        [SerializeField] protected string itemName;
        [SerializeField] protected EItemTier itemTier;
        [SerializeField] protected EItemType itemType;
        [SerializeField] protected List<Stat> stats;
        public List<Stat> Stats => stats;
        public EItemType ItemType => itemType;
        public Sprite ItemSprite => itemSprite;
        
        public abstract string GetItemName();
    }
}