using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace LRS.SceneManagement
{
    internal sealed class Settings : ScriptableObject
    {
        private static Settings _instance;

        private static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<Settings>("Settings");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<Settings>();
                        _instance.debugMode = false;
                        AssetDatabase.CreateAsset(_instance, "Assets/reliable-scene-manager/Resources/Settings.asset");
                    }
#endif
                }
                return _instance;
            }
        }

        [SerializeField, HideInInspector] private bool debugMode;
        [SerializeField, HideInInspector] private bool logEvents;
        [SerializeField, HideInInspector] private bool logSceneLoaded;
        [SerializeField, HideInInspector] private bool logSceneUnloaded;
        [SerializeField, HideInInspector] private bool logSceneSwitched;

        public static bool DebugMode
        {
            get => Instance.debugMode;
            set => Instance.debugMode = value;
        }
        
        public static bool LogEvents
        {
            get => Instance.logEvents && Instance.debugMode;
            set => Instance.logEvents = value;
        }
        
        public static bool LogSceneLoaded
        {
            get => Instance.logSceneLoaded && Instance.logEvents;
            set => Instance.logSceneLoaded = value;
        }
        
        public static bool LogSceneUnloaded
        {
            get => Instance.logSceneUnloaded && Instance.logEvents;
            set => Instance.logSceneUnloaded = value;
        }
        
        public static bool LogSceneSwitched
        {
            get => Instance.logSceneSwitched && Instance.logEvents;
            set => Instance.logSceneSwitched = value;
        }

        public static void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }
    }
}