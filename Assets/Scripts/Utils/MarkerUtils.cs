using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CustomAttributes;
using Enum;
using PlayerPack.PlayerBase;
using PlayerPack.PlayerOngoingStatsPack;
using ScriptableObjects;
using SoulPack;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Utils
{
    public class MarkerUtils
    {
        private readonly List<StatTuple> _baseMarkers = new();
        private readonly List<MockTuple> _mockValues = new();
        private readonly SoColorPalette _colorPalette;
        
        private string DefaultMarkerValue(EStatType statType) => GetColoredValue(GetValue(statType) + $"(<sprite name={statType}>100%)", statType);

        public MarkerUtils()
        {
            CreateBaseMarkers();

            _colorPalette = Resources.Load<SoColorPalette>("other/ColouredMarkers");
        }
        private void CreateMockData()
        {
            _mockValues.Clear();
            foreach (EStatType statType in System.Enum.GetValues(typeof(EStatType)))
            {
                var mockTuple = new MockTuple(statType, Random.Range(10, 25));
                _mockValues.Add(mockTuple);
            }
        }
        private void CreateBaseMarkers()
        {
            foreach (EStatType stat in System.Enum.GetValues(typeof(EStatType)))
            {
                var marker = CreateMarker(stat.ToString());
                var statTuple = new StatTuple(stat, marker);
                _baseMarkers.Add(statTuple);
            }
        }

        public string CreateMarker(string value)
        {
            var marker = string
                .Concat(value.Where(char.IsUpper)).ToLower();

            var suffix = GetSuffix(value, marker);
            marker += suffix;

            return "<>".Insert(1, marker);
        }

        private bool CheckForRepetition(string val)
        {
            val = "<>".Insert(1, val);
            return _baseMarkers.FirstOrDefault(t => t.value == val) != default;
        }

        private string GetSuffix(string stat, string marker)
        {
            var loweredStatName = stat.ToLower();
            var iteration = 0;
            var suffix = "";
            while (CheckForRepetition(marker + suffix))
            {
                if (iteration >= loweredStatName.Length)
                {
                    Debug.LogError("Failed to create marker!");
                    break;
                }
                    
                suffix = loweredStatName[^(iteration + 1)..];
                iteration++;
            }

            return suffix;
        }

        public List<StatTuple> GetStatMarkers()
        {
            return _baseMarkers;
        }

        public string PrepareDescription(string rawDescription, bool mockData)
        {
            if (mockData) CreateMockData();
            
            foreach (var statTuple in GetStatMarkers())
            {
                var marker = statTuple.value.RemoveSpecialCharacters();
                var statType = statTuple.key;
                var pattern = $@"<{marker}(?=[\s>])\s*.*?>";
                var regex = new Regex(pattern);
                rawDescription = regex.Replace(rawDescription, match => HandleReplacement(match.ToString(), statType));
            }

            return rawDescription;
        }

        private string HandleReplacement(string marker, EStatType statType)
        {
            const string ceilParamPattern = @"ceil";
            var ceil = marker.IndexOf(ceilParamPattern, StringComparison.Ordinal) != -1;
            var customStyle = HandleMarkerParam(marker);

            var retVal = HandlePercentageParam(marker, statType, ceil, customStyle);
            if (retVal != "") return retVal;

            retVal = HandleValueParam(marker, statType);
            if (retVal != "") return retVal;
            
            return DefaultMarkerValue(statType);
        }

        private string HandleValueParam(string marker, EStatType statType)
        {
            const string valueParamPattern = @"value\s*=\s*\d+";
            var regex = new Regex(valueParamPattern);
            if (!regex.IsMatch(marker)) return "";
            
            var match = regex.Match(marker).ToString();
            var numberValue = GetNumber(match);
                
            return numberValue == null ? DefaultMarkerValue(statType) : GetColoredValue(numberValue + $"(<sprite name={statType}>)", statType);
        }

        private EStatType? HandleMarkerParam(string marker)
        {
            const string styleMarkerParamPattern = @"marker\s*=\s*\p{L}+";
            var regex = new Regex(styleMarkerParamPattern);
            if (!regex.IsMatch(marker)) return null;
            
            var match = regex.Match(marker).ToString();
            const string markerPattern = @"=\s*\p{L}+";
            regex = new Regex(markerPattern);
            if (!regex.IsMatch(match)) return null;
            
            var matchedStatMarker = "<>".Insert(1,regex.Match(match).ToString().RemoveSpecialCharacters());
            var stat = _baseMarkers.FirstOrDefault(m => m.value == matchedStatMarker);
            return stat?.key;
        }

        private string HandlePercentageParam(string marker, EStatType statType, bool ceil, EStatType? customStyle)
        {
            const string percentageParamPattern = @"percentage\s*=\s*\d+";
            var regex = new Regex(percentageParamPattern);
            
            if (!regex.IsMatch(marker)) return "";
            var param = regex.Match(marker).ToString();
            var numberValue = GetNumber(param);
            if (numberValue == null) return DefaultMarkerValue(statType);
                
            var percentage = (float)numberValue / 100;
            var calculatedValueNumber = GetValue(statType) * percentage;
            if (ceil) calculatedValueNumber = Mathf.CeilToInt(calculatedValueNumber);
            var calculatedStringValue = calculatedValueNumber.ToString(CultureInfo.InvariantCulture);
            var readyValue = calculatedStringValue + GetColoredValue($"(<sprite name={statType}>{numberValue}%)", statType, true);

            return GetColoredValue(readyValue, customStyle ?? statType);
        }

        private static float? GetNumber(string param)
        {
            const string pattern = @"\b\d+\b";
            var regex = new Regex(pattern);
            if (!regex.IsMatch(param)) return null;
            
            return int.Parse(regex.Match(param).ToString());
        }

        private string GetColoredValue(string value, EStatType statType, bool baseColor = false)
        {
            var colour = _colorPalette.GetMarkerColour(statType);
            var baseColour = _colorPalette.BaseMarkerColor;
            return colour == "" ? (baseColor ? $"<color=#{ColorUtility.ToHtmlStringRGBA(baseColour)}>{value}</color>" : value) : $"<color=#{colour}>{value}</color>";
        }

        private float GetValue(EStatType statType)
        {
            if (_mockValues.Count == 0) return PlayerBase.Instance.Stats.GetStatValue(statType);
            var mockTuple = _mockValues.FirstOrDefault(v => v.key == statType);
            return mockTuple?.value ?? -1;
        }
    }

    [System.Serializable]
    public class CustomMarker
    {
        public string fieldName;
        public string marker;
        [ReadOnly] public float value;
    }

    public class StatTuple : Tuple<EStatType, string>
    {
        public StatTuple(EStatType statType, string marker) : base(statType, marker){}
    }

    public class MockTuple : Tuple<EStatType, float>
    {
        public MockTuple(EStatType statType, float statValue) : base(statType, statValue){}
    }
}