using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EntityBase.EntityBase), true)]
    public class CE_EntityBase : UnityEditor.Editor
    {
        private SerializedProperty _damageableType;
        private SerializedProperty _baseEntityStats;
        private SerializedProperty _baseEntityMS;
        private void OnEnable()
        {
            _baseEntityMS = serializedObject.FindProperty("baseEntityMS");
            _damageableType = serializedObject.FindProperty("damageableType");
            _baseEntityStats = serializedObject.FindProperty("baseEntityStats");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Main entity props", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_damageableType);
            if (_damageableType.enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(_baseEntityStats);
                EditorGUILayout.PropertyField(_baseEntityMS);
            }

            var childFields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                         BindingFlags.Public | BindingFlags.NonPublic);

            serializedObject.ApplyModifiedProperties();
            
            if (!childFields.Any(f => f.IsPublic || f.GetCustomAttribute(typeof(SerializeField)) != null)) return;
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Child entity props", EditorStyles.boldLabel);

            foreach (var field in childFields)
            {
                if (field.IsNotSerialized || field.IsStatic) continue;

                if (!field.IsPublic && field.GetCustomAttribute(typeof(SerializeField)) == null) continue;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name), true);
            }
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
    }
}