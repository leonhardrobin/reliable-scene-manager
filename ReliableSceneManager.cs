using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS.SceneManagement
{
    public class ReliableSceneManager
    {
        static ReliableSceneManager()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            DebugLog("Reliable Scene Manager Initialized");
        }

        public static bool debugMode = false;
        
        public static event Action<SceneReference> SceneLoaded;
        public static event Action<SceneReference> SceneUnloaded;
        public static event Action<SceneReference, SceneReference> ActiveSceneChanged;
        
        /// <summary>
        /// This is the current scene that is loaded.
        /// If multiple scenes are loaded, this will be the active scene.
        /// </summary>
        public static SceneReference CurrentScene { get; private set; }
        
        private static List<SceneReference> _sceneQueue = new();
        
        #region Scene Events
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DebugLog($"Scene Loaded: {scene.name}");
            SceneLoaded?.Invoke(SceneReferenceFrom(scene));
        }
        
        private static void OnSceneUnloaded(Scene scene)
        {
            DebugLog($"Scene Unloaded: {scene.name}");
            SceneUnloaded?.Invoke(SceneReferenceFrom(scene));
        }
        
        private static void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        {
            DebugLog($"Active Scene Changed: {previousScene.name} -> {newScene.name}");
            CurrentScene = SceneReferenceFrom(newScene);
            ActiveSceneChanged?.Invoke(SceneReferenceFrom(previousScene), CurrentScene);
        }
        
        #endregion
        
        #region Basic Scene Loading
        
        public static void LoadScene(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(scene.ScenePath, mode);
            
            // switch (mode)
            // {
            //     case LoadSceneMode.Single:
            //         CurrentScene = scene;
            //         break;
            //     case LoadSceneMode.Additive:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            // }
        }
        
        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, mode);
        }
        
        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }
        
        public static void LoadScene(SceneReference scene, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(scene.ScenePath, parameters);
            CurrentScene = scene;
        }
        
        public static void LoadScene(string sceneName, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneName, parameters);
            // switch (parameters.loadSceneMode)
            // {
            //     case LoadSceneMode.Single:
            //         CurrentScene = SceneReferenceFrom(sceneName);
            //         break;
            //     case LoadSceneMode.Additive:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(parameters.loadSceneMode), parameters.loadSceneMode, null);
            // }
        }
        
        public static void LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneBuildIndex, parameters);
        }
        
        public static AsyncOperation LoadSceneAsync(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ScenePath, mode);
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(SceneReference scene, LoadSceneParameters parameters)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ScenePath, parameters);
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneParameters parameters)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, parameters);
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneParameters parameters)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, parameters);
            return asyncOperation;
        }
        
        #endregion
        
        #region Basic Scene Unloading
        
        public static void UnloadScene(SceneReference scene)
        {
            SceneManager.UnloadSceneAsync(scene.ScenePath);
        }
        
        public static void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        
        public static void UnloadScene(int sceneBuildIndex)
        {
            SceneManager.UnloadSceneAsync(sceneBuildIndex);
        }
        
        public static AsyncOperation UnloadSceneAsync(SceneReference scene)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene.ScenePath);
            return asyncOperation;
        }
        
        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            return asyncOperation;
        }
        
        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex);
            return asyncOperation;
        }
        
        public static bool SetActiveAndCurrentScene(SceneReference scene)
        {
            bool value = false;
            if (scene.IsValid())
            {
                value = SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
            }
            return value;
        }
        
        #endregion
        
        #region Create Scene Reference
        
        public static SceneReference SceneReferenceFrom(Scene scene)
        {
            return new SceneReference(scene.path);
        }
        
        public static SceneReference SceneReferenceFrom(string sceneName)
        {
            return new SceneReference(sceneName);
        }
        
        public static SceneReference SceneReferenceFrom(int sceneBuildIndex)
        {
            return new SceneReference(SceneManager.GetSceneByBuildIndex(sceneBuildIndex).path);
        }
        
        #endregion
        
        #region Check Scene
        
        public static bool IsSceneLoaded(SceneReference scene)
        {
            return scene.IsLoaded();
        }
        
        public static bool IsSceneLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
        
        public static bool IsSceneLoaded(int sceneBuildIndex)
        {
            return SceneManager.GetSceneByBuildIndex(sceneBuildIndex).isLoaded;
        }
        
        public static bool IsSceneValid(SceneReference scene)
        {
            return scene.IsValid();
        }
        
        public static bool IsSceneValid(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).IsValid();
        }
        
        public static bool IsSceneValid(int sceneBuildIndex)
        {
            return SceneManager.GetSceneByBuildIndex(sceneBuildIndex).IsValid();
        }

        public static bool IsSceneActive(SceneReference scene)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.path == scene.ScenePath)
                {
                    return true;
                }
            }
            return false;
        }
        
        #endregion
        
        #region Scene List
        
        public static void AddSceneToQueue(SceneReference scene)
        {
            _sceneQueue.Add(scene);
        }
        
        public static void RemoveSceneFromQueue(SceneReference scene)
        {
            _sceneQueue.Remove(scene);
        }
        
        public static void ClearSceneQueue()
        {
            _sceneQueue.Clear();
        }
        
        /// <summary>
        /// Loads the next scene in the queue. If the current scene is not in the queue, it will load the first scene in the queue.  
        /// </summary>
        public static void LoadNextSceneInQueue()
        {
            if (_sceneQueue.Count <= 0)
            {
                Debug.LogWarning("No scenes in queue");
                return;
            }
            
            // if current scene is not in queue, index of will return -1 -> loading first scene in queue
            int index = _sceneQueue.IndexOf(CurrentScene) + 1;
            if (index < _sceneQueue.Count)
            {
                LoadScene(_sceneQueue[index]);
            }
        }
        
        /// <summary>
        /// Loads the next scene in the queue. If the current scene is not in the queue, it will load the first scene in the queue.
        /// Also sets the loaded scene as the active scene.
        /// </summary>
        /// <returns>The AsyncOperation loading the scene</returns>
        public static AsyncOperation LoadNextSceneInQueueAsync()
        {
            if (_sceneQueue.Count <= 0)
            {
                Debug.LogWarning("No scenes in queue");
                return null;
            }
            
            // if current scene is not in queue, index of will return -1 -> loading first scene in queue
            int index = _sceneQueue.IndexOf(CurrentScene) + 1;
            
            if (index >= _sceneQueue.Count) return null;
            
            SceneReference scene = _sceneQueue[index];
            AsyncOperation operation = LoadSceneAsync(scene);
            operation.completed += _ =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
            };
            return operation;
        }
        
        #endregion
        
        private static void DebugLog(string message)
        {
            if (debugMode)
            {
                Debug.Log(message);
            }
        }
    }
}
