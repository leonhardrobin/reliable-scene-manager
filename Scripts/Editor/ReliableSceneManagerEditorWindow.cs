#if UNITY_EDITOR
using System.Collections.Generic;
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
        private readonly string[] _tabs = {"Persistent Data", "Scene List", "Settings"};
        
        #endregion
        
        #region Scene List
        
        private List<SceneReference> _sceneList = new();
        private ReorderableList _reorderableList;
        
        private readonly Color _currentSceneColor = Color.green;
        private readonly Color _indexInSceneListColor = Color.yellow;
        
        #endregion
        
        [MenuItem("Window/LRS/Reliable Scene Manager")]
        public static void ShowWindow()
        {
            ReliableSceneManagerEditorWindow window = GetWindow<ReliableSceneManagerEditorWindow>("Reliable Scene Manager");
            window.minSize = new Vector2(MinWindowHeight, MinWindowWidth);
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

                    if (_sceneList[index] != null)
                    {
                        bool isCurrentScene = _sceneList[index] == ReliableSceneManager.CurrentScene && ReliableSceneManager.CurrentScene != null;
                        bool isIndexInSceneList = _sceneList[index] == _sceneList[ReliableSceneManager.IndexInSceneList] && _sceneList[ReliableSceneManager.IndexInSceneList] != null;
                        
                        if (isIndexInSceneList)
                        {
                            GUI.backgroundColor = _indexInSceneListColor;
                        }
                        if (isCurrentScene)
                        {
                            GUI.backgroundColor = _currentSceneColor;
                        }
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
                    if (list.index == ReliableSceneManager.IndexInSceneList && list.index != 0)
                    {
                        ReliableSceneManager.IndexInSceneList--;
                    }
                    _sceneList.RemoveAt(list.index);
                },
            };
        }

        private void OnGUI()
        {
            
            GUILayout.Space(10);
            
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs);
            
            GUILayout.Space(10);
            
            switch (_tabs[_selectedTab])
            {
                case "Persistent Data":
                    break;
                case "Scene List":
                    ReorderableList();
                    break;
                case "Settings":
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
            GUILayout.Label("Debugging", EditorStyles.boldLabel);
            
            Settings.DebugMode = EditorGUILayout.Toggle("Debug Mode", Settings.DebugMode);
        }
    }
}
#endif
