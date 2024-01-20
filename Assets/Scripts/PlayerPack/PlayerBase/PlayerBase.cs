using System;
using UnityEngine;

namespace PlayerPack.PlayerBase
{
    public class PlayerBase : EntityBase.EntityBase
    {
        public static PlayerBase Instance { get; private set; }
        public float PlayerMS => MovementSpeed;
        protected override void Awake()
        {
            base.Awake();
            
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
    }
}