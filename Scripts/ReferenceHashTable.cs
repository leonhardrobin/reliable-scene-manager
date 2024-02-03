using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace LRS.SceneManagement
{
    internal abstract unsafe class ReferenceHashTable<T> where T : unmanaged
    {
        private const short DataSize = 5000;

        private struct Data
        {
            public string Key;
            public T* DataPointer;
        }

        private static readonly Data[] DataArray = new Data[DataSize];

        private static readonly List<string> Keys = new();

        private static int Hash(string key)
        {
            key = key.ToLower();
            int hash = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hash = (hash << 5) - hash + key[i];
            }

            return hash % DataSize;
        }

        private static bool Insert(string key, T* dataPtr)
        {
            if (Keys.Contains(key))
            {
                Logger.LogWarning($"Key '{key}' already exists in the persistent data manager. Skipping insertion.");
                return false;
            }

            int index = Hash(key);
            if (DataArray[index].Key == null)
            {
                DataArray[index].Key = key;
                DataArray[index].DataPointer = dataPtr;
                Keys.Add(key);
                return true;
            }

            Logger.LogWarning($"Hash collision occurred for key '{key}'.\n" +
                              $"Try using a different key");
            return false;
        }

        private static bool Find(string key, out T* dataPtr)
        {
            if (!Keys.Contains(key))
            {
                Logger.LogWarning($"Key '{key}' does not exist in the persistent data manager.");
                dataPtr = null;
                return false;
            }

            int index = Hash(key);
            if (DataArray[index].Key == key)
            {
                dataPtr = DataArray[index].DataPointer;
                return true;
            }

            dataPtr = null;
            return false;
        }

        private static bool Remove(string key)
        {
            int index = Hash(key);
            if (DataArray[index].Key != key || !Keys.Contains(key))
            {
                Logger.LogWarning($"Key '{key}' does not exist in the persistent data manager.");
                return false;
            }

            DataArray[index].Key = null;
            DataArray[index].DataPointer = null;
            Keys.Remove(key);
            return true;
        }

        public static void SetData(string key, ref T obj)
        {
            T* dataPtr = (T*)UnsafeUtility.AddressOf(ref obj);
            if (Insert(key, dataPtr))
            {
                Logger.Log($"Data for key '{key}' has been persisted.");
            }
        }

        public static bool TryGetData(string key, out T obj)
        {
            if (Find(key, out T* dataPtr))
            {
                obj = *dataPtr;
                return true;
            }

            obj = default;
            return false;
        }

        public static ref T GetData(string key)
        {
            if (Find(key, out T* obj))
            {
                return ref *obj;
            }
            
            return ref UnsafeUtility.AsRef<T>(null);
        }

        public static void RemoveData(string key)
        {
            if (Remove(key))
            {
                Logger.Log($"Data for key '{key}' has been removed.");
            }
        }
    }
}