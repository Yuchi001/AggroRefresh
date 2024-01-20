using System;
using System.Linq;
using Enum;
using SoulPack;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(SoBaseEntityStats))]
    public class CE_SoBaseEntityStats : UnityEditor.Editor
    {
        private SoBaseEntityStats _baseEntityStats;

        private void OnEnable()
        {
            _baseEntityStats = target as SoBaseEntityStats;
        }

        public override void OnInspectorGUI()
        {
            foreach (EStatType statType in System.Enum.GetValues(typeof(EStatType)))
            {
                var stat = GetStat(statType);
                if(stat.statType is EStatType.Health) continue;
                
                EditorGUILayout.LabelField(stat.statType.ToString(), EditorStyles.boldLabel);
                stat.statValue = EditorGUILayout.FloatField("Value", stat.statValue);
                EditorGUILayout.Separator();
            }

            EditorUtility.SetDirty(_baseEntityStats);

            serializedObject.ApplyModifiedProperties();
        }

        private Stat GetStat(EStatType statType)
        {
            var stat = _baseEntityStats.StatList.FirstOrDefault(s => s.statType == statType);
            if (stat != default)
                return stat;

            stat = new Stat()
            {
                statType = statType,
                statValue = 0,
            };
            _baseEntityStats.StatList.Add(stat);
            return stat;
        }
    }
}