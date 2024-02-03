using LRS.Utils;
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

        
        // if the value is a gameobject or component on one then we mark it as DontDestroyOnLoad
        
        public static void Set<T>(string key, T value)
        {

            if (value is Object obj)
            {

                UnityMessageCallbacks callbacks = obj switch
                {
                    GameObject go => go.GetOrAdd<UnityMessageCallbacks>(),
                    Component component => component.gameObject.GetOrAdd<UnityMessageCallbacks>(),
                    _ => null
                };

                DontDestroyOnLoad(callbacks);
                Instance.objects[key] = obj;
                
            }
            else
            {
                Instance.data[key] = value;
            }
        }
        
        public static void UnSet(string key)
        {
            if (Instance.objects.ContainsKey(key))
            {
                Object obj = Instance.objects[key];
                if (obj is GameObject go)
                {
                    UnityMessageCallbacks callbacks = go.GetOrAdd<UnityMessageCallbacks>();
                    Destroy(callbacks);
                }
                else if (obj is Component component)
                {
                    UnityMessageCallbacks callbacks = component.gameObject.GetOrAdd<UnityMessageCallbacks>();
                    Destroy(callbacks);
                }
                else
                {
                    return;
                }
            }
            else
            {
                Instance.data.Remove(key);
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
}