using System;
using UnityEngine;

namespace Managers
{
    public class ExpManager : MonoBehaviour
    {
        [SerializeField] private float a;
        [SerializeField] private float b;
        
        public static ExpManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        public int GetExpNeeded(int level)
        {
            return Mathf.CeilToInt((float)(a * Math.Pow(level, b)));
        }

        public int GetCurrentLevel(int exp)
        {
            return Mathf.FloorToInt((float)Math.Pow(exp / a, 1.0 / b));
        }
    }
}