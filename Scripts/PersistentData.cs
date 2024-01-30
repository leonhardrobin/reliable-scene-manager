using UnityEditor;
using UnityEngine;

namespace LRS.SceneManagement
{
    internal class PersistentData : ScriptableObject
    {
        private static PersistentData _instance;

        private static PersistentData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<PersistentData>("PersistentData");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<PersistentData>();
                        AssetDatabase.CreateAsset(_instance,
                            "Assets/reliable-scene-manager/Resources/PersistentData.asset");
                    }
#endif
                }

                return _instance;
            }
        }

        private readonly SerializableDictionary<string, object> _data = new();
        
        public static void Set<T>(string key, T value) => Instance._data[key] = value;

        public static T Get<T>(string key)
        {
            if (Instance._data.ContainsKey(key))
            {
                return (T) Instance._data[key];
            }

            return default;
        }
    }
}