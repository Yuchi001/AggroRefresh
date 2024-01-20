using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AbilityPack.Enum;
using CustomAttributes;
using UnityEngine;
using Utils;

namespace AbilityPack
{
    public abstract class SoAbilityProps : ScriptableObject
    {
        [SerializeField] protected string abilityName;
        [SerializeField, TextArea] public string abilityDescription;
        [SerializeField, ReadOnly, TextArea] private string abilityDescriptionPreview;
        [SerializeField] private int abilityDisplayIndex;
        [SerializeField] protected Sprite abilityIcon;
        [SerializeField] protected Sprite continuesIcon;
        [SerializeField] protected bool isContinues;
        [SerializeField] protected bool isUsable;
        [SerializeField] protected float coolDown;
        [SerializeField] protected EAbilityType abilityType;
        
        public float AbilityCooldown => coolDown;
        public int AbilityDisplayIndex => abilityDisplayIndex;
        public Sprite AbilityIcon => abilityIcon;
        public Sprite ContinuesIcon => continuesIcon;
        public EAbilityType AbilityType => abilityType;
        public string AbilityName => abilityName;

        public struct AbilityInfoPack
        {
            public float Cooldown;
            public EAbilityType Type;
            public bool IsContinues;
            public Sprite Icon;
        }
        public delegate void UseAbilityDelegate(AbilityInfoPack abilityInfoPack);
        public static event UseAbilityDelegate OnUseAbility;

        public void UseAbility(EntityBase.EntityBase source)
        {
            AbilityInfoPack pack = new()
            {
                Cooldown = coolDown,
                Icon = abilityIcon,
                Type = AbilityType,
                IsContinues = isContinues,
            };
            OnUseAbility?.Invoke(pack);
            OnAbilityTrigger(source);
        }
        protected abstract void OnAbilityTrigger(EntityBase.EntityBase source);

        public string GetDescription(MarkerUtils markerUtils, bool mockData = false)
        {
            var customMarkers = GetCustomMarkers(markerUtils);

            var description = customMarkers.Aggregate(abilityDescription, 
                (current, marker) => current.Replace(marker.marker, 
                    marker.value.ToString(CultureInfo.InvariantCulture)));
            return markerUtils.PrepareDescription(description, mockData);
        }

        public List<CustomMarker> GetCustomMarkers(MarkerUtils markerUtils)
        {
            var customMarkers = new List<CustomMarker>();
            var childMarkerFields = GetMarkerValues(this);

            foreach (var fieldInfo in childMarkerFields)
            {
                var marker = new CustomMarker
                {
                    value = fieldInfo.fieldValue,
                    fieldName = fieldInfo.fieldName
                };
                marker.marker = markerUtils.CreateMarker(marker.fieldName);
                customMarkers.Add(marker);
            }
            
            var cooldownMarker = new CustomMarker()
            {
                value = GetCooldownMarkerValue(this).fieldValue,
                fieldName = "coolDown"
            };
            cooldownMarker.marker = markerUtils.CreateMarker(cooldownMarker.fieldName.FirstCharToUpper());

            customMarkers.Add(cooldownMarker);
            
            return customMarkers;
        }

        private static List<(string fieldName, float fieldValue)> GetMarkerValues(object obj)
        {
            var type = obj.GetType();

            var childProps = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                            BindingFlags.Public | BindingFlags.NonPublic);

            return childProps.Select(field => (field.Name.FirstCharToUpper(), Convert.ToSingle(field.GetValue(obj)))).ToList();
        }

        private static (string fieldName, float fieldValue) GetCooldownMarkerValue(object obj)
        {
            const string cooldownFieldName = "coolDown";
            var type = obj.GetType().BaseType;

            if (type == null) return (cooldownFieldName, 0);
            
            var fields = type.GetFields(BindingFlags.NonPublic |
                                        BindingFlags.Instance);

            var cooldownField = fields.FirstOrDefault(f => f.Name == cooldownFieldName);

            return cooldownField == default ? (cooldownFieldName, 0) : 
                (cooldownFieldName, Convert.ToSingle(cooldownField.GetValue(obj)));
        }

        public static bool operator !=(SoAbilityProps a1, SoAbilityProps a2)
        {
            if (a1 is null && a2 is null) return false;

            if (a1 is null || a2 is null) return true;
            
            return a1.abilityName != a2.abilityName || a1.abilityType != a2.AbilityType;
        }

        public static bool operator ==(SoAbilityProps a1, SoAbilityProps a2)
        {
            if (a1 is null && a2 is null) return true;

            if (a1 is null || a2 is null) return false;
            
            return a1.abilityName == a2.abilityName && a1.abilityType == a2.AbilityType;
        }
    }
}