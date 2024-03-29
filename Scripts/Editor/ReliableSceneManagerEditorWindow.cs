#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace LRS.SceneManagement.Editor
{
    public class ReliableSceneManagerEditorWindow : EditorWindow
    {
        private const int MinWindowWidth = 500;
        private const int MinWindowHeight = 300;
        
        #region Tabs 
        
        private int _selectedTab;
        private readonly string[] _tabs = {"Persistent Data", "Scene Queue", "Settings"};
        
        #endregion
        
        #region Scene List
        
        private List<SceneReference> _sceneQueue = new();
        private ReorderableList _reorderableList;
        
        private readonly Color _currentSceneColor = Color.green;
        private readonly Color _indexInSceneQueueColor = Color.yellow;
        
        #endregion
        
        [MenuItem("Window/LRS/Reliable Scene Manager")]
        public static void ShowWindow()
        {
            ReliableSceneManagerEditorWindow window = GetWindow<ReliableSceneManagerEditorWindow>("Reliable Scene Manager");
            window.minSize = new Vector2(MinWindowHeight, MinWindowWidth);
        }
        
        private void OnEnable()
        {
           SetUpReorderableList();
        }

        private void OnGUI()
        {
            
            GUILayout.Space(10);
            
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs, GUILayout.Height(30));
            
            GUILayout.Space(10);
            
            switch (_tabs[_selectedTab])
            {
                case "Persistent Data":
                    PersistentDataTab();
                    break;
                case "Scene Queue":
                    SceneQueueTab();
                    break;
                case "Settings":
                    SettingsTab();
                    break;
            }
        }
        
        private static void PersistentDataTab()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("Persistent Objects", EditorStyles.boldLabel);
                GUILayout.Space(10);
                
                foreach (KeyValuePair<string, Object> pair in PersistentDataManager.Objects)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key, EditorStyles.whiteLabel);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(pair.Value.name);
                    GUILayout.EndHorizontal();
                }
                
                
                GUILayout.Space(30);

                GUILayout.Label("Persistent Values", EditorStyles.boldLabel);
                GUILayout.Space(10);
                
                List<string> keys = PersistentDataManager.GetValueKeys<int>().ToList();
                keys.AddRange(PersistentDataManager.GetValueKeys<float>());
                keys.AddRange(PersistentDataManager.GetValueKeys<double>());
                keys.AddRange(PersistentDataManager.GetValueKeys<short>());
                keys.AddRange(PersistentDataManager.GetValueKeys<long>());
                keys.AddRange(PersistentDataManager.GetValueKeys<byte>());
                keys.AddRange(PersistentDataManager.GetValueKeys<bool>());
                keys.AddRange(PersistentDataManager.GetValueKeys<char>());
                keys.AddRange(PersistentDataManager.GetValueKeys<decimal>());
                
                
                foreach (string key in keys)
                {
                    GUILayout.Label(key, EditorStyles.whiteLabel);
                }
                
            }
            else
            {
                GUILayout.Label("Persistent Objects and Values are only available during play mode.", EditorStyles.boldLabel);
            }
            
        }

        private void SceneQueueTab()
        {
            _reorderableList.DoLayoutList();
            
            GUIStyle style = new (GUI.skin.button)
            {
                active = {textColor = Color.red},
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Debugging", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = _currentSceneColor;
            if (GUILayout.Button($"Show Current: {ReliableSceneManager.CurrentScene?.Name}",
                    new GUIStyle(GUI.skin.button)
                    {
                        normal = { textColor = _currentSceneColor },
                        active = { textColor = _currentSceneColor },
                        hover = { textColor = _currentSceneColor }
                    },
                    GUILayout.Width(150), GUILayout.Height(30)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(ReliableSceneManager.CurrentScene?.Path, typeof(SceneAsset)));
            }
            GUI.backgroundColor = _indexInSceneQueueColor;
            if (GUILayout.Button($"Load Next: {ReliableSceneManager.GetNextSceneInQueue()?.Name}",
                    new GUIStyle(GUI.skin.button)
                    {
                        normal = { textColor = _indexInSceneQueueColor },
                        active = { textColor = _indexInSceneQueueColor },
                        hover = { textColor = _indexInSceneQueueColor }
                    },
                    GUILayout.Width(150), GUILayout.Height(30)))
            {
                ReliableSceneManager.LoadNextSceneInQueueAsync();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
        }

        private void SetUpReorderableList()
        {
            _sceneQueue = SceneQueue.Scenes;
            _reorderableList = new ReorderableList(_sceneQueue, typeof(SceneReference), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Scene Queue");
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    SceneAsset oldScene = _sceneQueue[index] == null ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(_sceneQueue[index].Path);

                    if (_sceneQueue[index] != null)
                    {
                        bool isCurrentScene = _sceneQueue[index] == ReliableSceneManager.CurrentScene && ReliableSceneManager.CurrentScene != null;
                        bool isNextIndexInSceneQueue =
                            ReliableSceneManager.NextIndexInSceneQueue < _sceneQueue.Count && 
                            _sceneQueue[index] == _sceneQueue[ReliableSceneManager.NextIndexInSceneQueue] && 
                            _sceneQueue[ReliableSceneManager.NextIndexInSceneQueue] != null;

                        if (Application.isPlaying)
                        {
                            if (isNextIndexInSceneQueue)
                            {
                                GUI.backgroundColor = _indexInSceneQueueColor;
                            }
                            if (isCurrentScene)
                            {
                                GUI.backgroundColor = _currentSceneColor;
                            }
                        }
                    }
                    SceneAsset newScene = EditorGUI.ObjectField(rect, oldScene, typeof(SceneAsset), false) as SceneAsset;
                    GUI.backgroundColor = Color.white;
                    if (newScene == null)
                    {
                        _sceneQueue[index] = null;
                    }
                    else
                    {
                        _sceneQueue[index] = new SceneReference(AssetDatabase.GetAssetPath(newScene));
                        ReliableSceneManager.AddSceneToBuildSettings(_sceneQueue[index]);
                    }
                    
                    SceneQueue.Save();
                },
                onAddCallback = _ =>
                {
                    _sceneQueue.Add(null);
                },
                onRemoveCallback = list =>
                {
                    if (list.index == ReliableSceneManager.NextIndexInSceneQueue && list.index != 0)
                    {
                        ReliableSceneManager.NextIndexInSceneQueue--;
                    }
                    _sceneQueue.RemoveAt(list.index);
                    
                    SceneQueue.Save();
                },
                onChangedCallback = _ =>
                {
                    SceneQueue.Save();
                },
            };
        }

        private static void SettingsTab()
        {
            GUILayout.Label("Debugging", EditorStyles.boldLabel);
            
            Settings.DebugMode = EditorGUILayout.Toggle("Debug Mode", Settings.DebugMode);
            
            GUILayout.Space(10);
            
            GUILayout.Label("Events", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(!Settings.DebugMode);
            
            Settings.LogEvents = EditorGUILayout.Toggle("Log Events", Settings.LogEvents);
            
            EditorGUI.BeginDisabledGroup(!Settings.LogEvents);
            
            Settings.LogSceneLoaded = EditorGUILayout.Toggle("Log Scene Loaded", Settings.LogSceneLoaded);
            Settings.LogSceneUnloaded = EditorGUILayout.Toggle("Log Scene Unloaded", Settings.LogSceneUnloaded);
            Settings.LogSceneSwitched = EditorGUILayout.Toggle("Log Scene Switched", Settings.LogSceneSwitched);
            
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            
            if (GUI.changed)
            {
                Settings.Save();
            }
        }
    }
}
#endif
