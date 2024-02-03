using LRS.Singleton;
using LRS.Utils;
using UnityEngine;

namespace LRS.SceneManagement
{
    internal class PersistentDataManager : PersistentSingleton<PersistentDataManager>
    {
        [SerializeField, HideInInspector] private SerializableDictionary<string, Object> objects = new();
        [SerializeField, HideInInspector] private SerializableDictionary<string, object> data = new();
        
        public static void Set<T>(string key, T value)
        {
            if (Instance.objects.ContainsKey(key) || Instance.data.ContainsKey(key))
            {
                Logger.LogWarning($"Key '{key}' already exists in the persistent data manager. Overwriting the value.");
            }
            
            if (value is Object obj)
            {
                UnityMessageCallbacks callbacks = obj switch
                {
                    GameObject go => go.GetOrAdd<UnityMessageCallbacks>(),
                    Component component => component.gameObject.GetOrAdd<UnityMessageCallbacks>(),
                    _ => null
                };

                if (callbacks != null)
                {
                    callbacks.onDestroy += _ =>
                    {
                        if (Application.isPlaying)
                            Instance.objects.Remove(key);
                    };
                    
                    DontDestroyOnLoad(callbacks);
                }

                Instance.objects[key] = obj;
            }
            else
            {
                Instance.data[key] = value;
            }
        }
        
        public static bool TryGet<T>(string key, out T value)
        {
            if (Instance.data.ContainsKey(key))
            {
                value = (T) Instance.data[key];
                return true;
            }

            if (Instance.objects.ContainsKey(key))
            {
                if (Instance.objects[key] is T obj)
                {
                    value = obj;
                    return true;
                }
            }

            value = default;
            return false;
        }
        
        public static T Get<T>(string key)
        {
            if (Instance.data.ContainsKey(key))
            {
                return (T) Instance.data[key];
            }

            if (Instance.objects.ContainsKey(key))
            {
                if (Instance.objects[key] is T obj)
                {
                    return obj;
                }
            }
            
            return default;
        }
    }
}