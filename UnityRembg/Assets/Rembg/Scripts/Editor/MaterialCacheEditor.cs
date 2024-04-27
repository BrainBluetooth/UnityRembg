using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace BrainBluetooth.Rembg
{
    [CustomEditor(typeof(MaterialCache))]
    public class MaterialCacheEditor : Editor
    {
        private static readonly FieldInfo fieldDic = typeof(MaterialCache).GetField("dic", (BindingFlags)(-1));

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var com = base.target as MaterialCache;
            var dic = fieldDic.GetValue(com) as Dictionary<string, Material>;
            foreach (var pair in dic)
            {
                EditorGUILayout.ObjectField(new GUIContent(pair.Key), pair.Value, typeof(Material), false);
            }
        }
    }
}