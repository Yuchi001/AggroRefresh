using System;
using SoulPack.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [FormerlySerializedAs("soulManagementPanel")]
        [Header("Prefabs")]
        [SerializeField] private GameObject soulManagementPanelPrefab;
        [SerializeField] private GameObject soulInfoPanelPrefab;
        
        [Header("Public fields")]
        [SerializeField] private AnimationCurve toggleUiAlphaCurve;
        [SerializeField] private AnimationCurve toggleUiScaleCurve;
        [FormerlySerializedAs("mainCanvas")] [SerializeField] private Transform mainCanvasTransform;
        [FormerlySerializedAs("gameCanvas")] [SerializeField] private Transform gameCanvasTransform;
        public AnimationCurve ToggleUiAlphaCurve => toggleUiAlphaCurve;
        public AnimationCurve ToggleUiScaleCurve => toggleUiScaleCurve;
        public Transform MainCanvasTransform => mainCanvasTransform;
        public Transform GameCanvasTransform => gameCanvasTransform;
        public Canvas MainCanvas => mainCanvasTransform.GetComponent<Canvas>();
        public Canvas GameCanvas => gameCanvasTransform.GetComponent<Canvas>();
        public RectTransform MainCanvasRect => mainCanvasTransform.GetComponent<RectTransform>();

        public static UiManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
    }
}