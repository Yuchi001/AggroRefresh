using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AbilityPack;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(SoAbilityProps), true)]
    public class CE_SoAbilityProps : UnityEditor.Editor
    {
        private MarkerUtils _markerUtils;
        private SoAbilityProps _script;
        private List<StatTuple> _statMarkers;
        private Texture _copyIcon;
        private bool _showDescriptionSettings = true;
        private bool _showBasicMarkers = true;
        private bool _showCustomMarkers = true;

        // SerializedProps
        private SerializedProperty _name;
        private SerializedProperty _description;
        private SerializedProperty _descriptionPreview;
        private SerializedProperty _abilitySoulAssociate;
        private SerializedProperty _abilityDisplayIndex;
        private SerializedProperty _icon;
        private SerializedProperty _isContinues;
        private SerializedProperty _isUsable;
        private SerializedProperty _coolDown;
        private SerializedProperty _continuesIcon;
        private SerializedProperty _unlockLevel;
        private SerializedProperty _abilityType;
        private SerializedProperty _abilityBehaviour;
        private SerializedProperty _setIdManually;

        private void OnEnable()
        {
            _script = target as SoAbilityProps;
            _markerUtils = new MarkerUtils();
            _statMarkers = _markerUtils.GetStatMarkers();
            _copyIcon = Resources.Load("icons/icons8-copy-30") as Texture;
            _name = serializedObject.FindProperty("abilityName");
            _icon = serializedObject.FindProperty("abilityIcon");
            _isContinues = serializedObject.FindProperty("isContinues");
            _isUsable = serializedObject.FindProperty("isUsable");
            _coolDown = serializedObject.FindProperty("coolDown");
            _continuesIcon = serializedObject.FindProperty("continuesIcon");
            _description = serializedObject.FindProperty("abilityDescription");
            _descriptionPreview = serializedObject.FindProperty("abilityDescriptionPreview");
            _abilityType = serializedObject.FindProperty("abilityType");
            _abilitySoulAssociate = serializedObject.FindProperty("abilitySoulAssociate");
            _abilityDisplayIndex = serializedObject.FindProperty("abilityDisplayIndex");
            _unlockLevel = serializedObject.FindProperty("unlockLevel");
            _abilityBehaviour = serializedObject.FindProperty("abilityBehaviour");
            _setIdManually = serializedObject.FindProperty("setIdManually");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_name);
            EditorGUILayout.PropertyField(_icon);

            EditorGUILayout.PropertyField(_isUsable);
            if (_isUsable.boolValue)
            {
                EditorGUILayout.PropertyField(_isContinues);
            }

            if (_isContinues.boolValue && _isUsable.boolValue)
            {
                EditorGUILayout.PropertyField(_continuesIcon);
            }

            EditorGUILayout.PropertyField(_coolDown);

            HandleChildProps();

            _showCustomMarkers = EditorGUILayout.BeginFoldoutHeaderGroup(_showCustomMarkers, "Custom markers");
            if (_showCustomMarkers) HandleCustomMarkers();
            EditorGUILayout.EndFoldoutHeaderGroup();

            _showBasicMarkers = EditorGUILayout.BeginFoldoutHeaderGroup(_showBasicMarkers, "Basic markers");
            if (_showBasicMarkers) HandleMarkers();
            EditorGUILayout.EndFoldoutHeaderGroup();

            _showDescriptionSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(_showDescriptionSettings, "Description settings");
            if (_showDescriptionSettings) HandleDescription();
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.PropertyField(_abilityType);
            EditorGUILayout.PropertyField(_abilitySoulAssociate);
            EditorGUILayout.PropertyField(_abilityBehaviour);

            HandleDisplayIndex();
            
            serializedObject.ApplyModifiedProperties();
            
            Repaint();
        }

        private void HandleChildProps()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Child ability props", EditorStyles.boldLabel);
            var childFields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                         BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in childFields)
            {
                if (field.IsNotSerialized || field.IsStatic) continue;

                if (!field.IsPublic && field.GetCustomAttribute(typeof(SerializeField)) == null) continue;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name), true);
            }
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        private void HandleDisplayIndex()
        {
            _setIdManually.boolValue = EditorGUILayout.ToggleLeft("Set ID manually", _setIdManually.boolValue);
            EditorGUILayout.PropertyField(_unlockLevel);
            EditorGUI.BeginDisabledGroup(!_setIdManually.boolValue);
            EditorGUILayout.PropertyField(_abilityDisplayIndex);
            EditorGUI.EndDisabledGroup();
            _abilityDisplayIndex.intValue =
                _setIdManually.boolValue ? _abilityDisplayIndex.intValue : _unlockLevel.intValue;
        }

        private void HandleDescription()
        {
            var descriptionHeight = CalculateTextFieldHeight(_description);
            var descriptionPreviewHeight = CalculateTextFieldHeight(_descriptionPreview);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_description, GUILayout.Height(descriptionHeight));
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                _descriptionPreview.stringValue = _script.GetDescription(_markerUtils, true);
            }

            EditorGUILayout.PropertyField(_descriptionPreview, GUILayout.Height(descriptionPreviewHeight));
        }

        private static float CalculateTextFieldHeight(SerializedProperty property)
        {
            var areaStyle = new GUIStyle(GUI.skin.textArea);
            areaStyle.wordWrap = true;
            var width = EditorGUIUtility.currentViewWidth - (1 / EditorGUIUtility.currentViewWidth) - 150;
            areaStyle.fixedHeight = 0;
            areaStyle.fixedHeight = areaStyle.CalcHeight(new GUIContent(property.stringValue), width);
            return areaStyle.fixedHeight < 100 ? 100 : areaStyle.fixedHeight;
        }

        private void HandleMarkers()
        {
            for (var i = 0; i < _statMarkers.Count; i += 2)
            {
                GUILayout.BeginHorizontal();
                DisplayMarker(_statMarkers[i]);
                if (_statMarkers.Count > i + 1)
                    DisplayMarker(_statMarkers[i + 1]);
                GUILayout.EndHorizontal();
            }
        }

        private void HandleCustomMarkers()
        {
            var customMarkers = _script.GetCustomMarkers(_markerUtils);
            for (var i = 0; i < customMarkers.Count; i += 2)
            {
                GUILayout.BeginHorizontal();
                DisplayCustomMarker(customMarkers[i]);
                if (customMarkers.Count > i + 1)
                    DisplayCustomMarker(customMarkers[i + 1]);
                GUILayout.EndHorizontal();
            }
        }

        private void DisplayMarker(StatTuple statTuple)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.BeginVertical(GUILayout.Width(60));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField(statTuple.key.ToString());
            EditorGUILayout.TextField(statTuple.value);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();
            HandleButton(statTuple.value);
            GUILayout.EndHorizontal();
        }
        
        private void DisplayCustomMarker(CustomMarker customMarker)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.BeginVertical(GUILayout.Width(60));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField(customMarker.fieldName);
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField(customMarker.marker);
            EditorGUILayout.TextField(customMarker.value.ToString(CultureInfo.InvariantCulture));
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();
            HandleButton(customMarker.marker);
            GUILayout.EndHorizontal();
        }

        private void HandleButton(string copyVal)
        {
            var width = GUILayout.Width(35);
            var height = GUILayout.Height(35);
            if (GUILayout.Button(_copyIcon, width, height))
                EditorGUIUtility.systemCopyBuffer = copyVal;
        }
    }
}