using System;
using Managers;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(ExpManager))]
    public class CE_ExpManager : UnityEditor.Editor
    {
        private SerializedProperty _aSerializedProperty;
        private SerializedProperty _bSerializedProperty;

        private int exp = 1;
        private int lvl = 1;

        private int expNeeded = 0;
        private int currentLvl = 0;

        private ExpManager _script;
        private void OnEnable()
        {
            _script = target as ExpManager;
            _aSerializedProperty = serializedObject.FindProperty("a");
            _bSerializedProperty = serializedObject.FindProperty("b");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_aSerializedProperty);
            EditorGUILayout.PropertyField(_bSerializedProperty);

            EditorGUILayout.LabelField("Test options", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Exp needed for max lvl", _script.GetExpNeeded(100));
            EditorGUILayout.IntField("Exp needed for lvl 50", _script.GetExpNeeded(50));
            EditorGUILayout.IntField("Exp needed for lvl 10", _script.GetExpNeeded(10));
            EditorGUILayout.IntField("Exp needed for lvl 1", _script.GetExpNeeded(1));
            EditorGUI.EndDisabledGroup();
            lvl = EditorGUILayout.IntSlider("Level", lvl, 1, 100);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Exp needed for next level", _script.GetExpNeeded(lvl));
            EditorGUI.EndDisabledGroup();
            exp = EditorGUILayout.IntSlider("Exp", exp, 1, 10000);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Calculated level", _script.GetCurrentLevel(exp));
            EditorGUI.EndDisabledGroup();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}