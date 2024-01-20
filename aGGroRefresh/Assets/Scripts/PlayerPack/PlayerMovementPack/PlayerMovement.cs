using System.Collections.Generic;
using AbilityPack.Enum;
using Other;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using Utils;

namespace PlayerPack.PlayerMovementPack
{
    public partial class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Transform shootPos;

        private PlayerBase.PlayerBase PlayerBaseStats => PlayerBase.PlayerBase.Instance;
        public Vector3 ShootPos => shootPos.position;
        private static PlayerOngoingStats OngoingStats => PlayerOngoingStats.Instance;
        private PlayerOptions _playerOptions;

        private Material _material => GetComponent<SpriteRenderer>().material;

        private void Awake()
        {
            _playerOptions = GameManager.Instance.GetPlayerOptions();
            _buttonsActive = new Dictionary<KeyCode, bool>
            {
                { UpBind, false },
                { LeftBind, false },
                { DownBind, false },
                { RightBind, false },
            };
        }

        private void Update()
        {
            ManageMovement();
        }

        private void ManageMovement()
        {
            //todo: check for stunes!

            //UtilsMethods.LookAtMouse(transform);
            
            transform.rotation = Quaternion.Euler(0, transform.position.x > UtilsMethods.GetMousePosition().x ? 180: 0, 0);

            var movement = GetMovement();
            var position = transform.position;
            position += (Vector3)movement;

            transform.position = position;
        }
    }
}