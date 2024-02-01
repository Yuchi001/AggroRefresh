using System;
using System.Collections.Generic;
using ItemPack.ScriptableObjects;
using Other.Interfaces;
using PlayerPack.PlayerBase;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using UnityEngine.UI;

namespace ItemPack
{
    public class ItemPrefab : MonoBehaviour, IObstacle
    {
        [SerializeField] private SpriteRenderer itemSprite;
        [SerializeField] private GameObject itemGUI;
        [SerializeField] private float itemPickUpRange;
        [SerializeField] private AnimationCurve movementCurve;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float detectObstacleRange;

        [SerializeField] private bool useDebugItem;
        [SerializeField] private SoItem debugItem;

        public float ForcedMovementSpeed => movementSpeed * Time.deltaTime;

        private GameObject _spawnedGUI = null;

        private Vector2 _goToPos;
        private float _startDistance;
        
        private void Awake()
        {
            if (!useDebugItem) return;
            Setup(debugItem);
        }

        private SoItem _item;

        public void Setup<T>(T item, Vector3? position = null) where T: SoItem
        {
            _item = item;
            itemSprite.sprite = item.ItemSprite;

            var currentPos = transform.position;
            _goToPos = position ?? currentPos;
            _startDistance = position != null ? Vector2.Distance(currentPos, position.Value) : 0;
        }

        private void DisplayGUI()
        {
            if (_item == null || _spawnedGUI != null || itemGUI == null) return;

            _spawnedGUI = Instantiate(itemGUI);
        }

        private void Update()
        {
            MoveToTargetPosition();
            
            if (PlayerBase.Instance == null) return;

            if (Vector2.Distance(PlayerBase.Instance.transform.position, transform.position) > itemPickUpRange)
            {
                _spawnedGUI = null;
                return;
            }
            
            DisplayGUI();

            if (!Input.GetKeyDown(KeyCode.F)) return;
            
            var pickedUp = PlayerOngoingStats.Instance.PickUpItem(_item);
            if (pickedUp) Destroy(gameObject);
        }

        private void MoveToTargetPosition()
        {
            var currentDist = Vector2.Distance(transform.position, _goToPos);
            if (_startDistance == 0 || currentDist <= 0.1f) return;
            
            var lerp = 1 - currentDist / _startDistance;
            var currentMovementSpeed = movementSpeed * movementCurve.Evaluate(lerp);
            transform.position = Vector2.MoveTowards(
                transform.position, 
                _goToPos, 
                currentMovementSpeed * Time.deltaTime);

            var hitObjArr = Array.Empty<Collider2D>();
            Physics2D.OverlapCircleNonAlloc(transform.position, detectObstacleRange, hitObjArr);
            if(hitObjArr.Length != 0)Debug.Log(hitObjArr.Length);
            foreach (var hitObj in hitObjArr)
            {
                if (!hitObj.transform.TryGetComponent(out IObstacle obstacle)) continue;

                var moved = obstacle.TryMove(transform.position);
                if(moved) continue;

                _startDistance = 0;
            }
        }

        public bool TryMove(Vector3 sourceObjectPosition)
        {
            var direction = (transform.position - sourceObjectPosition).normalized;
            var invertedDirection = direction * -1;
            var hitObjArr = Array.Empty<RaycastHit2D>();
            Physics2D.RaycastNonAlloc(transform.position, invertedDirection, hitObjArr, detectObstacleRange);
            Debug.Log(hitObjArr.Length);
            foreach (var hitObj in hitObjArr)
            {
                if (!hitObj.transform.TryGetComponent(out IObstacle obstacle)) continue;

                var moved = obstacle.TryMove(transform.position);
                if(moved) continue;

                return false;
            }

            transform.position =
                Vector2.MoveTowards(transform.position, 
                    transform.position + invertedDirection, 
                    1);

            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, itemPickUpRange);
            Gizmos.DrawWireSphere(transform.position, detectObstacleRange);
        }
    }
}