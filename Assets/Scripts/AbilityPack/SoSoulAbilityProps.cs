using SoulPack.Enum;
using UnityEngine;

namespace AbilityPack
{
    [CreateAssetMenu(menuName = "Custom/Ability/SoulAbility", fileName = "new Soul Ability")]
    public abstract class SoSoulAbilityProps : SoAbilityProps
    {
        [SerializeField] protected ESoulType abilitySoulAssociate;
        
        public ESoulType AbilitySoulAssociate => abilitySoulAssociate;
    }
}