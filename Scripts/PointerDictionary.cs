using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

namespace LRS.SceneManagement
{
    internal abstract unsafe class PointerDictionary<T> where T : unmanaged
    {
        public struct PointerWrapper
        {
            public T* Pointer;
        }

        public static readonly Dictionary<string, PointerWrapper> Pointers = new();

        public static bool Add(string key, ref T value)
        {
            T* pointer = (T*)UnsafeUtility.AddressOf(ref value);
            return Pointers.TryAdd(key, new PointerWrapper {Pointer = pointer});
        }
        
        public static bool TryGet(string key, out T value)
        {
            if (Pointers.TryGetValue(key, out PointerWrapper pointerWrapper))
            {
                value = *pointerWrapper.Pointer;
                return true;
            }

            value = default;
            return false;
        }
        
        public static ref T Get(string key)
        {
            return ref *Pointers[key].Pointer;
        }
        
        public static bool RemoveData(string key)
        {
            return Pointers.Remove(key);
        }
        
        public static void Clear()
        {
            Pointers.Clear();
        }
        
        public static Dictionary<string, T> ToDictionary()
        {
            return Pointers.ToDictionary(pair => pair.Key, pair => *pair.Value.Pointer);
        }
    }
}