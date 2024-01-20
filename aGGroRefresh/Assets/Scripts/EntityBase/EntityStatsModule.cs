using System;
using System.Collections.Generic;
using System.Linq;
using AbilityPack.Enum;
using Enum;
using Interfaces;
using JetBrains.Annotations;
using Managers.Enums;
using SoulPack;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerPack.PlayerOngoingStatsPack
{
    public class EntityStatsModule : MonoBehaviour
    {
        private SoBaseEntityStats _baseStats;

        private EntityBase.EntityBase _entityBaseObject = null;
        private readonly List<StatModifier> _statModifiers = new();
        private readonly List<SpecialEffectModifier> _specialEffects = new();

        public void Setup(EntityBase.EntityBase entityBase, SoBaseEntityStats baseEntityStats)
        {
            _entityBaseObject = entityBase;
            _baseStats = baseEntityStats;
            foreach (var stat in _baseStats.StatList)
            {
                ModifyStat(stat.statType, stat.statValue);
            }
            ModifyStat(EStatType.Health, GetStatValue(EStatType.MaxHealth));
        }
        private void Update()
        {
            foreach (var modifier in _statModifiers.Where(modifier => modifier.LastingDuration != null))
            {
                modifier.LastingDuration -= Time.deltaTime;
            }
            _statModifiers.RemoveAll(m => m.LastingDuration is <= 0);
            
            foreach (var effect in _specialEffects)
            {
                ResolveEffect(effect);
            }
            _specialEffects.RemoveAll(e => e.EffectPack.EffectDurationTime <= 0);
        }

        private void ResolveEffect(SpecialEffectModifier effect)
        {
            effect.EffectPack.EffectDurationTime -= Time.deltaTime;
            if (effect.Period != null)
            {
                effect.Period -= Time.deltaTime;
            }

            if (effect.Period is null or > 0) return;

            effect.Period = CalculatePeriod(effect.EffectPack);
            switch (effect.EffectPack.SpecialEffectType)
            {
                case ESpecialEffectType.Burn:
                    var burnDamagePack = new IDamageable.DamagePack()
                    {
                        DamageType = EDamageType.Magic,
                        OnHitDamage = effect.EffectPack.EffectValue,
                    };
                    _entityBaseObject.Hit(burnDamagePack, EParticlesType.Burn);
                    return;
                case ESpecialEffectType.Poison:
                    var poisonDamagePack = new IDamageable.DamagePack()
                    {
                        DamageType = EDamageType.Magic,
                        OnHitDamage = effect.EffectPack.EffectValue,
                    };
                    _entityBaseObject.Hit(poisonDamagePack, EParticlesType.Poison);
                    return;
                case ESpecialEffectType.Bleed:
                    var bleedDamagePack = new IDamageable.DamagePack()
                    {
                        DamageType = EDamageType.Magic,
                        OnHitDamage = effect.EffectPack.EffectValue,
                    };
                    _entityBaseObject.Hit(bleedDamagePack);
                    return;
                default:
                    Debug.LogError($"Special effect of type: {effect.EffectPack.SpecialEffectType} is not implemented");
                    return;
            }
        }

        private static float? CalculatePeriod(IDamageable.SpecialEffectPack specialEffectPack)
        {
            if (specialEffectPack.EffectFrequency == null) return null;

            var f = specialEffectPack.EffectFrequency;
            f = f == 0 ? float.MaxValue : f;
            return 1f / f;
        }
        

        public void AddSpecialEffect(IDamageable.SpecialEffectPack specialEffectPack)
        {
            //todo: rewrite special effects
        }

        private void UseInstantEffect(IDamageable.SpecialEffectPack specialEffectPack)
        {
            // for now there isnt any effect that needs custom implementation, but that might change in future
        }

        public void RemoveAllSpecialEffects(ESpecialEffectType? specialEffectType = null)
        {
            if (specialEffectType == null)
            {
                _specialEffects.Clear();
                return;
            }

            _specialEffects.RemoveAll(s => s.EffectPack.SpecialEffectType == specialEffectType.Value);
        }
        
        public void RemoveAllSpecialEffects(string effectId)
        {
            _specialEffects.RemoveAll(s => s.EffectPack.EffectId == effectId);
        }

        public void ModifyStat(EStatType statType, float modifiedVal, float? durationTime = null)
        {
            var statModifier = new StatModifier()
            {
                Value = modifiedVal,
                LastingDuration = durationTime,
                StatType = statType,
            };
            _statModifiers.Add(statModifier);
        }

        public float GetStatValue(EStatType statType)
        {
            return _statModifiers.All(sm => sm.StatType != statType) ? 0 : 
                _statModifiers.Where(sm => sm.StatType == statType).Sum(sm => sm.Value);
        }

        public void RemoveAllStatsWithId(string statId)
        {
            _statModifiers.RemoveAll(s => s.StatId == statId);
        }

        #region static methods

        private static bool IsSpecialEffectStackable(ESpecialEffectType effectType)
        {
            return effectType is ESpecialEffectType.Burn or ESpecialEffectType.Poison or ESpecialEffectType.Slow;
        }
        
        private static bool IsSpecialEffectInstant(ESpecialEffectType effectType)
        {
            return effectType is ESpecialEffectType.Slow or ESpecialEffectType.Stun;
        }

        private bool IsEffectActive(ESpecialEffectType effectType)
        {
            return _specialEffects.Count(e => e.EffectPack.SpecialEffectType == effectType) != 0;
        }

        private static bool IsEffectAStatModifier(ESpecialEffectType effectType)
        {
            return effectType is ESpecialEffectType.Slow;
        }

        #endregion
    }

    public class StatModifier
    {
        public EStatType StatType;
        public float Value;
        public float? LastingDuration;
        [CanBeNull] public string StatId;
    }

    public class SpecialEffectModifier
    {
        public IDamageable.SpecialEffectPack EffectPack;
        public float? Period;
    }
}