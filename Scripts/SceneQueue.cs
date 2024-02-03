using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace LRS.SceneManagement
{
    internal sealed class SceneQueue : ScriptableObject
    {
        private static SceneQueue _instance;

        private static SceneQueue Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SceneQueue>("SceneQueue");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<SceneQueue>();
                        _instance.scenes = new List<SceneReference>();
                        AssetDatabase.CreateAsset(_instance,
                            "Assets/reliable-scene-manager/Resources/SceneQueue.asset");
                    }
#endif
                }
                
                return _instance;
            }
        }

        [SerializeField, HideInInspector] private List<SceneReference> scenes = new();

        public static List<SceneReference> Scenes
        {
            get => Instance.scenes;
            set => Instance.scenes = value;
        }

        public static int Count => Instance.scenes.Count;

        public static void Clear() => Instance.scenes.Clear();

        public static void Add(SceneReference scene) => Instance.scenes.Add(scene);

        public static void Remove(SceneReference scene) => Instance.scenes.Remove(scene);

        public static void RemoveAt(int index) => Instance.scenes.RemoveAt(index);

        public static void Insert(int index, SceneReference scene) => Instance.scenes.Insert(index, scene);

        public static int IndexOf(SceneReference scene) => Instance.scenes.IndexOf(scene);

        public static void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }
    }
}