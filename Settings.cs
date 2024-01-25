using UnityEditor;
using UnityEngine;

namespace LRS.SceneManagement
{
    public class Settings : ScriptableObject
    {
        private static Settings _instance;

        private bool _debugMode;

        private static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<Settings>("Settings");
                    if (_instance == null)
                    {
                        _instance = CreateInstance<Settings>();
                        _instance._debugMode = false;
                        AssetDatabase.CreateAsset(_instance, "Assets/reliable-scene-manager/Resources/Settings.asset");
                    }
                }
                return _instance;
            }
        }
        
        public static bool DebugMode
        {
            get => Instance._debugMode;
            set => Instance._debugMode = value;
        }
    }
}