using System;
using System.Collections.Generic;
using System.Linq;
using AbilityPack.Enum;
using Interfaces;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulPack.UI.Combat
{
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private List<Utils.Tuple<EDamageType, Color>> damageColors;
        [SerializeField] private Vector2 spawnOffset;
        [SerializeField] private float shrinkSpeed;
        [SerializeField] private Vector2 movementSpeed;
        [SerializeField] private float maxMovementHeight;
        
        private Vector2 _sourceObjectPos;
        private bool SourceOnLeft => _sourceObjectPos.x < transform.position.x;
        private Vector2 _startPos;
        private bool _reachedMaxHeight = false;
        private Collider2D _hitObjCollider;
        
        public void Setup(IDamageable.DamagePack damagePack, Collider2D hitObjCollider2D)
        {
            _sourceObjectPos = hitObjCollider2D.transform.position;
            
            if (damagePack.OnHitDamage == null) return;

            foreach (Transform child in transform)
            {
                if(!child.TryGetComponent<TextMeshProUGUI>(out var tmPro)) continue;
                tmPro.text = damagePack.OnHitDamage.ToString();
                
                if(child.GetSiblingIndex() == 0) continue;
                tmPro.color = damageColors.FirstOrDefault(c => c.key == damagePack.DamageType)!.value;
            }

            var bounds = hitObjCollider2D.bounds;
            _startPos = transform.position = new Vector3()
            {
                x = _sourceObjectPos.x + Random.Range(-bounds.size.x, bounds.size.x) + Random.Range(-spawnOffset.x, spawnOffset.x),
                y = _sourceObjectPos.y + bounds.size.y / 2f + Random.Range(-spawnOffset.y, spawnOffset.y),
                z = 0,
            };
        }

        private void Update()
        {
            var shrinkModifier = shrinkSpeed * Time.deltaTime;
            transform.localScale -= new Vector3(shrinkModifier, shrinkModifier, shrinkModifier);
            transform.position += new Vector3()
            {
                x = movementSpeed.x * Time.deltaTime * (SourceOnLeft ? 1 : -1),
                y = movementSpeed.y * Time.deltaTime * (_reachedMaxHeight ? -1 : 1),
                z = 0,
            };
            
            if (Mathf.Abs(Vector2.Distance(transform.position, _startPos) - maxMovementHeight) <= 0.05f) 
                _reachedMaxHeight = true;
            
            if(transform.localScale.x <= 0) Destroy(gameObject);
        }
    }
}