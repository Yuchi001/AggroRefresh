using AbilityPack;
using ItemPack.ScriptableObjects;
using UnityEngine;
using Utils;
using WeaponPack.Enum;

namespace WeaponPack.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Custom/Item/Weapon", fileName = "new Weapon")]
    public sealed class SoWeapon : SoEqItem
    {
        [SerializeField] private EWeaponType weaponType;
        [SerializeField] private SoWeaponAbility primaryAttack;

        public void Shoot(EntityBase.EntityBase source)
        {
            primaryAttack.UseAbility(source);
        }

        public override string GetItemName()
        {
            return $"{itemTier.ToString().FirstCharToUpper()} {itemName.ToLower()}";
        }
    }
}