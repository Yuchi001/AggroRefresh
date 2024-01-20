using System;
using System.Collections;
using AbilityPack.Enum;
using Enum;
using Interfaces;
using Managers;
using Managers.Enums;
using PlayerPack.PlayerOngoingStatsPack;
using SoulPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityBase
{
    public class EntityBase : MonoBehaviour, IDamageable
    {
        private static readonly float HitTime = 0.1f;
        private static readonly int HitProperty = Shader.PropertyToID("_Hit");
        private static readonly int AngleProperty = Shader.PropertyToID("_Angle");
        private Collider2D Collider2D => GetComponent<Collider2D>();
        private Material Material => GetComponent<SpriteRenderer>().material;

        [SerializeField] private float baseEntityMS = 300;
        [SerializeField] private EDamageableType damageableType;
        [SerializeField] private SoBaseEntityStats baseEntityStats;
        public EntityStatsModule Stats { get; private set; }
        protected float MovementSpeed => (baseEntityMS + Stats.GetStatValue(EStatType.Dexterity)) / 100f;
        public EDamageableType DamageableType => damageableType;

        private const int ModifyShadowAngle = 0;

        protected virtual void Awake()
        {
            if (damageableType == EDamageableType.None) return;
            
            Stats = new GameObject(gameObject.name + "_StatsModule").AddComponent<EntityStatsModule>();
            Stats.enabled = true;
            Stats.transform.SetParent(transform);
            Stats.Setup(this, baseEntityStats);
        }

        public virtual void Hit(IDamageable.DamagePack damagePack, EParticlesType hitParticlesType = EParticlesType.Blood)
        {
            Material.SetInt(HitProperty, 1);
            StartCoroutine(RemoveHitEffect());
            if(damagePack.SpecialEffect != null) Stats.AddSpecialEffect(damagePack.SpecialEffect);

            if (damagePack.OnHitDamage == null) return;

            Stats.ModifyStat(EStatType.Health, -damagePack.OnHitDamage.Value);

            ParticlesManager.Instance.SpawnDamageIndicator(damagePack, Collider2D);

            if (Stats.GetStatValue(EStatType.Health) <= 0)
            {
                Die(damagePack.Source, GetDeathCause(hitParticlesType));
                return;
            }
            
            var (pos, rot) = GetPosAndRot(damagePack.Source);
            ParticlesManager.Instance.SpawnParticles(hitParticlesType, pos, rot);
        }

        private ESpecialEffectType? GetDeathCause(EParticlesType particlesType)
        {
            if (particlesType == EParticlesType.Blood) return null;

            switch (particlesType)
            {
                case EParticlesType.Burn:
                    return ESpecialEffectType.Burn;
                case EParticlesType.Poison:
                    return ESpecialEffectType.Poison;
                default:
                    Debug.LogError($"Death cause not implemented for {particlesType} particles type.");
                    return null;
            }
        }

        public void Die(GameObject source, ESpecialEffectType? causeOfDeath = null)
        {
            var (pos, rot) = GetPosAndRot(source);
            var sourceNotNull = source != null;
            if (causeOfDeath is null)
            {
                ParticlesManager.Instance.SpawnParticles(
                    sourceNotNull ? EParticlesType.Death : EParticlesType.DeathNoSource, 
                    transform.position, 
                    rot);
                Destroy(gameObject);
                return;
            }

            switch (causeOfDeath)
            {
                case ESpecialEffectType.Burn:
                    ParticlesManager.Instance.SpawnParticles(
                        EParticlesType.DeathBurn, 
                        transform.position, 
                        rot);
                    break;
                case ESpecialEffectType.Poison:
                    ParticlesManager.Instance.SpawnParticles(
                        EParticlesType.DeathPoison, 
                        transform.position, 
                        rot);
                    break;
            }
            Destroy(gameObject);
        }

        private (Vector2 pos, Quaternion rot) GetPosAndRot(GameObject source)
        {
            if (source == null) return (pos: GetRandomColliderPoint(), rot: Quaternion.identity);

            var damageSourcePos = source.transform.position;
            return (pos: Collider2D.ClosestPoint(damageSourcePos),
                rot: Quaternion.FromToRotation(transform.position, damageSourcePos));
        }

        private Vector2 GetRandomColliderPoint()
        {
            var boundsSize = Collider2D.bounds.size;
            var currentPosX = transform.position.x;
            var currentPosY = transform.position.y;
            return new Vector2()
            {
                x = Random.Range(currentPosX - boundsSize.x / 2, currentPosX + boundsSize.x / 2),
                y = Random.Range(currentPosY - boundsSize.y / 2, currentPosY + boundsSize.y / 2),
            };
        }

        protected virtual void Update()
        {
            Material.SetFloat(AngleProperty, 
                (transform.eulerAngles.z + 
                 transform.eulerAngles.y + 
                 ModifyShadowAngle) * Mathf.Deg2Rad);
        }
        
        private IEnumerator RemoveHitEffect()
        {
            yield return new WaitForSeconds(HitTime);
            Material.SetInt(HitProperty, 0);
        }
    }
}