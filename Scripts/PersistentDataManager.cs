using System;
using System.Collections.Generic;
using LRS.Singleton;
using LRS.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LRS.SceneManagement
{
    internal class PersistentDataManager : PersistentSingleton<PersistentDataManager>
    {
        public static readonly Dictionary<string, Object> Objects = new();

        public static void PersistObject<T>(string key, T value) where T : Object
        {
            if (Objects.ContainsKey(key))
            {
                Logger.LogWarning($"Key '{key}' already exists in the persistent data manager. Overwriting the value.");
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
                    if (HasInstance && Objects.ContainsKey(key))
                        Objects.Remove(key);
                };

                DontDestroyOnLoad(callbacks.gameObject);
            }
            else
            {
                Logger.LogWarning($"Object is not of type GameObject or Component. ({typeof(T).Name})" +
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

            Logger.LogWarning($"Key '{key}' does not exist in the persistent data manager." +
                              $"\nUse {nameof(ReliableSceneManager.TryGetPersistedObject)} instead.");

            return default;
        }

        public static void PersistValue<T>(string key, ref T value) where T : unmanaged
        {
            ReferenceHashTable<T>.SetData(key, ref value);
        }
        
        public static void PersistValue(string key, ref string value)
        {
            throw new NotImplementedException();
        }

        public static ref T GetPersistedValue<T>(string key) where T : unmanaged
        {
            return ref ReferenceHashTable<T>.GetData(key);
        }
        
        public static string GetPersistedValue(string key)
        {
            throw new NotImplementedException();
        }
        
        public static bool TryGetPersistedValue<T>(string key, out T value) where T : unmanaged
        {
            return ReferenceHashTable<T>.TryGetData(key, out value);
        }
        
        public static bool TryGetPersistedValue(string key, out string value)
        {
            throw new NotImplementedException();
        }
        
        public static void RemovePersistedValue<T>(string key) where T : unmanaged
        {
            ReferenceHashTable<T>.RemoveData(key);
        }
        
        public static void RemovePersistedValue(string key)
        {
            throw new NotImplementedException();
        }
        
        public static List<string> GetKeysOfValues()
        {
            return ReferenceHashTable<int>.Keys;
        }
    }
}