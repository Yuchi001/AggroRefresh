using System;
using ItemPack.ScriptableObjects;
using PlayerPack.PlayerBase;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using UnityEngine.UI;

namespace ItemPack
{
    public class ItemPrefab : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer itemSprite;
        [SerializeField] private GameObject itemGUI;
        [SerializeField] private float itemPickUpRange;

        [SerializeField] private bool useDebugItem;
        [SerializeField] private SoItem debugItem;

        private GameObject _spawnedGUI = null;
        
        private void Awake()
        {
            if (!useDebugItem) return;
            Setup(debugItem);
        }

        private SoItem _item;

        public void Setup<T>(T item) where T: SoItem
        {
            _item = item;
            itemSprite.sprite = item.ItemSprite;
        }

        private void DisplayGUI()
        {
            if (_item == null || _spawnedGUI != null || itemGUI == null) return;

            _spawnedGUI = Instantiate(itemGUI);
        }

        private void Update()
        {
            if (PlayerBase.Instance == null) return;

            if (Vector2.Distance(PlayerBase.Instance.transform.position, transform.position) > itemPickUpRange)
            {
                _spawnedGUI = null;
                return;
            }
            
            DisplayGUI();

            if (!Input.GetKeyDown(KeyCode.F)) return;
            
            PlayerOngoingStats.Instance.PickUpItem(_item);
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, itemPickUpRange);
        }
    }
}