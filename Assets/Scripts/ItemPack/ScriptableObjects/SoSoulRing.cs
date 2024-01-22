using AbilityPack;
using SoulPack.Enum;
using Unity.VisualScripting;
using UnityEngine;

namespace ItemPack.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Custom/Item/SoulRing", fileName = "new Basic Soul Ring")]
    public sealed class SoSoulRing : SoEqItem
    {
        [SerializeField] private ESoulType soulType;
        [SerializeField] private SoSoulAbilityProps soulAbility;
        [SerializeField] private int soulPointScaler;

        public override string GetItemName()
        {
            return
                $"{itemTier.ToString().FirstCharacterToUpper()} {soulType.ToString().ToLower()} {itemName.ToString().ToLower()}";
        }
    }
}