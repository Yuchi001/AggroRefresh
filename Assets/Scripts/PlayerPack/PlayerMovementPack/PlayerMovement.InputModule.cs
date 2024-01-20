using System.Collections.Generic;
using System.Linq;
using Enum;
using UnityEngine;
using Utils;

namespace PlayerPack.PlayerMovementPack
{
    public partial class PlayerMovement
    {
        private Dictionary<KeyCode, bool> _buttonsActive;
        private KeyCode UpBind => _playerOptions.upKeyBind;
        private KeyCode DownBind => _playerOptions.downKeyBind;
        private KeyCode LeftBind => _playerOptions.leftKeyBind;
        private KeyCode RightBind => _playerOptions.rightKeyBind;

        private float MovementSpeed => PlayerBaseStats.PlayerMS;

        public delegate void UseAbilityDelegate();
        public static event UseAbilityDelegate OnInputAbility;

        public delegate void ShootDelegate();
        public static event ShootDelegate OnShootInput;

        public delegate void DashDelegate();
        public static event DashDelegate OnDashInput;

        public delegate void UseSecondAbilityDelegate();
        public static event UseSecondAbilityDelegate OnInputSecondAbility;

        private Vector2 GetMovement()
        {
            var movement = Vector2.zero;

            CheckKey(UpBind, DownBind);
            CheckKey(LeftBind, RightBind);

            movement.x = _buttonsActive[RightBind] ? 1 : (_buttonsActive[LeftBind] ? -1 : 0);
            movement.y = _buttonsActive[UpBind] ? 1 : (_buttonsActive[DownBind] ? -1 : 0);

            if (movement.x != 0 && movement.y != 0)
                movement *= MovementSpeed / Mathf.Sqrt(2) * Time.deltaTime;
            else
                movement *= MovementSpeed * Time.deltaTime;

            return movement;
        }

        private void CheckKey(KeyCode main, KeyCode opposite)
        {
            if (Input.GetKeyDown(main))
            {
                _buttonsActive[main] = true;
                _buttonsActive[opposite] = false;
            }
            if (Input.GetKeyDown(opposite))
            {
                _buttonsActive[opposite] = true;
                _buttonsActive[main] = false;
            }

            if (Input.GetKeyUp(main))
            {
                if (Input.GetKey(opposite))
                {
                    _buttonsActive[opposite] = true;
                }
                _buttonsActive[main] = false;
            }

            if (Input.GetKeyUp(opposite))
            {
                if (Input.GetKey(main))
                {
                    _buttonsActive[main] = true;
                }
                _buttonsActive[opposite] = false;
            }
        }
    }
}