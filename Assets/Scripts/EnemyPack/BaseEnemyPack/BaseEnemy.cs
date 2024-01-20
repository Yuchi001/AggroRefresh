using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace EnemyPack.BaseEnemyPack
{
    public class BaseEnemy : EntityBase.EntityBase
    {
        private void DisplayDamage(int damage)
        {
            Debug.Log(damage);
        }
    }
} 