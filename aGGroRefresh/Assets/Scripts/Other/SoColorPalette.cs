using System.Collections.Generic;
using System.Linq;
using Enum;
using SoulPack.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "new Palette", menuName = "Custom/Palette", order = 0)]
    public class SoColorPalette : ScriptableObject
    {
        [Header("Souls palette")] 
        [SerializeField] private List<SoulColors> soulColorPackages = new();

        [Header("Markers palette")]
        [SerializeField] private List<Utils.Tuple<EStatType, string>> colouredMarkers;
        [SerializeField] private Color baseMarkerColor;

        public Color BaseMarkerColor => baseMarkerColor;
        
        public string GetMarkerColour(EStatType statType)
        {
            var markerTuple = colouredMarkers.FirstOrDefault(m => m.key == statType);
            return markerTuple == default ? "" : markerTuple.value;
        }
    }

    [System.Serializable]
    public struct SoulColors
    {
        public ESoulType soulType;
        public Color mainColor;
        public Color backgroundColor;
    }
}