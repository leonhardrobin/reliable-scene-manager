using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace LRS.SceneManagement.Editor
{
    public class ReliableSceneManagerEditorWindow : EditorWindow
    {
        #region Tabs 
        
        private int _selectedTab;
        private readonly string[] _tabs = {"Scene List", "Settings"};
        
        #endregion
        
        #region Scene List
        
        private List<SceneReference> _sceneList = new();
        private ReorderableList _reorderableList;
        
        #endregion
        
        [MenuItem("Window/LRS/Reliable Scene Manager")]
        public static void ShowWindow()
        {
            GetWindow<ReliableSceneManagerEditorWindow>("Reliable Scene Manager");
        }
        
        private void OnEnable()
        {
            _sceneList = SceneList.Scenes;
            _reorderableList = new ReorderableList(_sceneList, typeof(SceneReference), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Scene List");
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    SceneAsset oldScene = _sceneList[index] == null ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(_sceneList[index].Path);
                    
                    if (_sceneList[ReliableSceneManager.IndexInSceneList] == _sceneList[index])
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    else if (ReliableSceneManager.CurrentScene == _sceneList[index])
                    {
                        GUI.backgroundColor = Color.yellow;
                    }
                    SceneAsset newScene = EditorGUI.ObjectField(rect, oldScene, typeof(SceneAsset), false) as SceneAsset;
                    GUI.backgroundColor = Color.white;
                    _sceneList[index] = newScene == null ? null : new SceneReference(AssetDatabase.GetAssetPath(newScene));
                },
                onAddCallback = _ =>
                {
                    _sceneList.Add(null);
                },
                onRemoveCallback = list =>
                {
                    _sceneList.RemoveAt(list.index);
                },
            };
        }

        private void OnGUI()
        {
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs);
            
            GUILayout.Space(10);
            
            switch (_selectedTab)
            {
                case 0:
                    //SceneListTab();
                    ReorderableList();
                    break;
                case 1:
                    SettingsTab();
                    break;
            }
        }

        private void ReorderableList()
        {
            _reorderableList.DoLayoutList();
        }

        private static void SettingsTab()
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            
            GUILayout.Space(10);
            
            Settings.DebugMode = EditorGUILayout.Toggle("Debug Mode", Settings.DebugMode);
        }

        private void SceneListTab()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scene List", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Scene"))
            {
                _sceneList.Add(null);
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            List<SceneAsset> oldScenes = new (_sceneList.Count);

            foreach (SceneReference sceneReference in _sceneList)
            {
                SceneAsset oldScene = sceneReference == null ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneReference.Path);
                oldScenes.Add(oldScene);
            }

            EditorGUI.BeginChangeCheck();
            
            List<SceneAsset> newScenes = new (oldScenes.Count);
            
            for (int i = 0; i < _sceneList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SceneAsset newScene = EditorGUILayout.ObjectField(oldScenes[i], typeof(SceneAsset), false) as SceneAsset;
                if (GUILayout.Button("Remove"))
                {
                    //_sceneList.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                newScenes.Add(newScene);
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _sceneList = newScenes.Select(sceneAsset => new SceneReference(AssetDatabase.GetAssetPath(sceneAsset)))
                    .ToList();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Save"))
            {
                SaveSceneList();
            }
        }
        
        private void SaveSceneList()
        {
            ReliableSceneManager.ClearSceneQueue();
                
            foreach (SceneReference sceneReference in _sceneList)
            {
                if (sceneReference == null)
                {
                    Debug.LogWarning("Scene reference is null. Skipping.");
                    continue;
                }

                ReliableSceneManager.AddSceneToBuildSettings(sceneReference);
                ReliableSceneManager.AddSceneToQueue(sceneReference);
            }
        }
    }
}
