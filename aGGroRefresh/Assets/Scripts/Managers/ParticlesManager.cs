using System;
using System.Collections.Generic;
using System.Linq;
using AbilityPack.Enum;
using Interfaces;
using Managers.Enums;
using SoulPack.UI.Combat;
using UnityEngine;

namespace Managers
{
    public class ParticlesManager : MonoBehaviour
    {
        [SerializeField] private List<Utils.Tuple<EParticlesType, GameObject>> particles;
        [SerializeField] private GameObject damageIndicator;
        public static ParticlesManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void SpawnParticles(EParticlesType particlesType, Vector3 position, Quaternion rotation, float time = 2f, Transform parent = null)
        {
            var particlesTuple = particles.FirstOrDefault(p => p.key == particlesType);
            if (particlesTuple == default) return;

            var particleObject = Instantiate(particlesTuple.value, position, rotation, parent);
            Destroy(particleObject, time);
        }

        public void SpawnDamageIndicator(IDamageable.DamagePack damagePack, Collider2D hitObjCollider2D)
        {
            var damageIndicatorObj = Instantiate(damageIndicator, UiManager.Instance.GameCanvasTransform);
            damageIndicatorObj.GetComponent<DamageIndicator>().Setup(damagePack, hitObjCollider2D);
        }
    }
}