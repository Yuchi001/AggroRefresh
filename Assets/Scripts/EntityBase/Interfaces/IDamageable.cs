using AbilityPack.Enum;
using Enum;
using JetBrains.Annotations;
using Managers.Enums;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interfaces
{
    public interface IDamageable
    {
        public EntityStatsModule Stats { get; }
        public EDamageableType DamageableType { get; }
        
        public void Hit(DamagePack damagePack, EParticlesType hitParticlesType = EParticlesType.Blood);

        public void Die(GameObject source, ESpecialEffectType? causeOfDeath = null);

        public struct DamagePack
        {
            public GameObject Source;
            public int? OnHitDamage;
            public EDamageType DamageType;
            public SpecialEffectPack SpecialEffect;
        }

        public class SpecialEffectPack
        {
            public ESpecialEffectType SpecialEffectType;
            public int EffectValue;
            public float EffectDurationTime;
            public float? EffectFrequency;
            public int? Stacks;
            [CanBeNull] public string EffectId;
        }
    }
}