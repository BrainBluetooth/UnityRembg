using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainBluetooth.Rembg
{
    internal sealed class MaterialCache : MonoBehaviour
    {
        private static MaterialCache _instance;
        public static MaterialCache instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateInstance();
                return _instance;
            }
        }
        private static MaterialCache CreateInstance()
        {
            GameObject gObj = new GameObject("[Script]MaterialCache");
            DontDestroyOnLoad(gObj);
            MaterialCache com = gObj.AddComponent<MaterialCache>();
            return com;
        }

        private Dictionary<string, Material> dic;

        public static Material Get(string name)
        {
            var dic = instance.dic;

            Material mat;
            if (dic.TryGetValue(name, out mat))
            {
                if (mat == null)
                    Debug.LogWarning("Destroyed");
                return mat;
            }

            Shader shader = Shader.Find(name);
            if (shader == null)
                throw new ArgumentException();
            mat = new Material(shader);
            dic.Add(name, mat);

            return mat;
        }

        public static void Remove(string name)
        {
            var dic = instance.dic;

            if (dic.TryGetValue(name, out Material mat))
            {
                DestroyImmediate(mat);
                dic.Remove(name);
            }
        }

        public static void Clear()
        {
            var dic = instance.dic;

            foreach (Material mat in dic.Values)
                DestroyImmediate(mat);
            dic.Clear();
        }

        private void Awake()
        {
            dic = new Dictionary<string, Material>();
        }

        private void OnDestroy()
        {
            Clear();
            dic = null;
        }
    }
}