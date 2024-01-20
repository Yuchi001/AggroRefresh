using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace Utils.CustomAttributes.Editor
{
    public static class SerializeFieldExtensions
    {
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (!element.Contains("["))
                {
                    obj = GetValue_Imp(obj, element);
                    continue;
                }
                
                var elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
                var index = System.Convert.ToInt32(element[element.IndexOf("[", StringComparison.Ordinal)..].Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
 
            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);
 
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);
 
                type = type.BaseType;
            }
            return null;
        }
 
        private static object GetValue_Imp(object source, string name, int index)
        {
            if (GetValue_Imp(source, name) is not IEnumerable enumerable) return null;
            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}