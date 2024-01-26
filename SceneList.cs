using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LRS.SceneManagement
{
    internal sealed class SceneList : ScriptableObject
    {
        private static SceneList _instance;
        private List<SceneReference> _scenes = new();

        private static SceneList Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SceneList>("SceneList");
                    if (_instance == null)
                    {
                        _instance = CreateInstance<SceneList>();
                        _instance._scenes = new List<SceneReference>();
                        AssetDatabase.CreateAsset(_instance, "Assets/reliable-scene-manager/Resources/SceneList.asset");
                    }
                }
                return _instance;
            }
        }
        
        public static List<SceneReference> Scenes
        {
            get => Instance._scenes;
            set => Instance._scenes = value;
        }

        public static int Count => Instance._scenes.Count;
        
        public static void Clear() => Instance._scenes.Clear();
        
        public static void Add(SceneReference scene) => Instance._scenes.Add(scene);
        
        public static void Remove(SceneReference scene) => Instance._scenes.Remove(scene);
        
        public static void RemoveAt(int index) => Instance._scenes.RemoveAt(index);
        
        public static void Insert(int index, SceneReference scene) => Instance._scenes.Insert(index, scene);
        
        public static int IndexOf(SceneReference scene) => Instance._scenes.IndexOf(scene);
    }
}