using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

        [SerializeField, HideInInspector] private SerializableDictionary<string, Object> objects = new();

        [SerializeField, HideInInspector] private SerializableDictionary<string, object> data = new();

        public static void Set<T>(string key, T value)
        {
            bool isObject = value is Object;

            if (isObject)
            {
                Instance.objects[key] = value as Object;

                switch (value)
                {
                    case GameObject go:
                        var callbacks = go.GetOrAdd<UnityMessageCallbacks>();
                        callbacks.onDestroy += obj =>
                        {
                            Instance.objects[key] = Instantiate(go);
                        };
                        break;
                    case Component component:
                        break;
                }
            }
            else
            {
                Instance.data[key] = value;
            }
        }

        public static T GetObj<T>(string key) where T : Object
        {
            if (Instance.objects.ContainsKey(key))
            {
                return Instance.objects[key] as T;
            }

            return null;
        }

        public static T Get<T>(string key)
        {
            if (Instance.data.ContainsKey(key))
            {
                return (T) Instance.data[key];
            }

            return default;
        }
    }

    public static class Extensions
    {
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }
    }
}