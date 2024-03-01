using System;
using System.Collections.Generic;
using System.Linq;
using LRS.Utils;
using UnityEngine;
using static LRS.SceneManagement.Logger;
using Object = UnityEngine.Object;

namespace LRS.SceneManagement
{
    internal abstract class PersistentDataManager
    {
        public static readonly Dictionary<string, Object> Objects = new();

        public static void PersistObject<T>(string key, T value) where T : Object
        {
            if (Objects.ContainsKey(key))
            {
                LogWarning($"Key '{key}' already exists in the persistent data manager. Overwriting the value.");
            }

            UnityMessageCallbacks callbacks = value switch
            {
                GameObject go => go.GetOrAdd<UnityMessageCallbacks>(),
                Component component => component.gameObject.GetOrAdd<UnityMessageCallbacks>(),
                _ => null
            };

            if (callbacks != null)
            {
                callbacks.onDestroy += () =>
                {
                    if (!Application.isPlaying) return;
                    if (Objects.ContainsKey(key))
                        Objects.Remove(key);
                };

                Object.DontDestroyOnLoad(callbacks.gameObject);
            }
            else
            {
                LogWarning($"Object is not of type GameObject or Component. ({typeof(T).Name})" +
                                  $"\nObject will not be automatically removed from the persistent data manager.");
            }

            Objects[key] = value;
        }

        public static bool TryGetPersistedObject<T>(string key, out T value) where T : Object
        {
            if (Objects.TryGetValue(key, out Object obj))
            {
                value = obj as T;
                return true;
            }

            value = null;
            return false;
        }

        public static T GetPersistedObject<T>(string key) where T : Object
        {
            if (Objects.ContainsKey(key))
            {
                return Objects[key] is T ? (T)Objects[key] : null;
            }

            LogWarning($"Key '{key}' does not exist in the persistent data manager." +
                              $"\nUse {nameof(ReliableSceneManager.TryGetPersistedObject)} instead.");

            return default;
        }

        public static void PersistValue<T>(string key, ref T value) where T : unmanaged
        {
            if (PointerDictionary<T>.Add(key, ref value))
            {
                Log($"Persisted value '{key}' of type '{typeof(T).Name}'");
            }
            else
            {
                LogWarning($"Key '{key}' could not be added to the persistent data manager." +
                                  $"\nKey already exists in the persistent data manager.");
            }
        }
        
        public static void PersistValue(string key, ref string value)
        {
            throw new NotImplementedException();
        }

        public static ref T GetPersistedValue<T>(string key) where T : unmanaged
        {
            return ref PointerDictionary<T>.Get(key);
        }
        
        public static string GetPersistedValue(string key)
        {
            throw new NotImplementedException();
        }
        
        public static bool TryGetPersistedValue<T>(string key, out T value) where T : unmanaged
        {
            return PointerDictionary<T>.TryGet(key, out value);
        }
        
        public static bool TryGetPersistedValue(string key, out string value)
        {
            throw new NotImplementedException();
        }
        
        public static void RemovePersistedValue<T>(string key) where T : unmanaged
        {
            PointerDictionary<T>.RemoveData(key);
        }
        
        public static void RemovePersistedValue(string key)
        {
            throw new NotImplementedException();
        }
        
        public static Dictionary<string, T> GetValueDictionary<T>() where T : unmanaged
        {
            return PointerDictionary<T>.ToDictionary();
        }
        
        public static List<string> GetValueKeys<T>() where T : unmanaged
        {
            return PointerDictionary<T>.Pointers.Keys.ToList();
        }
    }
}