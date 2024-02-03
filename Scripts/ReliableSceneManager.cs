using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LRS.SceneManagement.Logger;

namespace LRS.SceneManagement
{
    public static class ReliableSceneManager
    {
        static ReliableSceneManager()
        {
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
            SceneManager.sceneUnloaded += OnSceneUnloadedHandler;
            SceneManager.activeSceneChanged += OnActiveSceneChangedHandler;
        }

        /// <summary>
        /// To get the current scene, use the <see cref="CurrentScene"/> property instead.
        /// </summary>
        public static event Action CurrentSceneChanged;

        /// <summary>
        /// This is the current scene that is loaded.
        /// If multiple scenes are loaded, this will be the active scene.
        /// </summary>
        public static SceneReference CurrentScene { get; private set; }

        internal static int IndexInSceneQueue { get; set; }

        #region Wrapper Properties and Fields for SceneManager

        public static event Action<SceneReference> SceneLoaded;
        public static event Action<SceneReference> SceneUnloaded;
        public static event Action<SceneReference, SceneReference> ActiveSceneChanged;
        public static int LoadedSceneCount => SceneManager.loadedSceneCount;
        public static int SceneCount => SceneManager.sceneCount;
        public static int SceneCountInBuildSettings => SceneManager.sceneCountInBuildSettings;

        #endregion

        #region Wrapper Methods for SceneManager

        #region Create Scene

        public static SceneReference CreateScene(string sceneName)
        {
            Scene scene = SceneManager.CreateScene(sceneName);
            return SceneReferenceFrom(scene);
        }

        public static SceneReference CreateScene(string sceneName, CreateSceneParameters parameters)
        {
            Scene scene = SceneManager.CreateScene(sceneName, parameters);
            return SceneReferenceFrom(scene);
        }

        #endregion

        #region Get Scene

        public static SceneReference GetActiveScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            return SceneReferenceFrom(scene);
        }


        public static SceneReference GetSceneAt(int index)
        {
            Scene scene = SceneManager.GetSceneAt(index);
            return SceneReferenceFrom(scene);
        }

        public static SceneReference GetSceneByBuildIndex(int buildIndex)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            return SceneReferenceFrom(scene);
        }

        public static SceneReference GetSceneByName(string name)
        {
            Scene scene = SceneManager.GetSceneByName(name);
            return SceneReferenceFrom(scene);
        }

        public static SceneReference GetSceneByPath(string scenePath)
        {
            Scene scene = SceneManager.GetSceneByPath(scenePath);
            return SceneReferenceFrom(scene);
        }

        #endregion

        #region Load Scene

        public static void LoadScene(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(scene.Name, mode);
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
            SceneManager.LoadScene(scene.Path, parameters);
            CurrentScene = scene;
        }

        public static void LoadScene(string sceneName, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneName, parameters);
        }

        public static void LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneBuildIndex, parameters);
        }

        public static AsyncOperation LoadSceneAsync(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.Path, mode);
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
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.Name, parameters);
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

        #region Unload Scene

        public static void UnloadScene(SceneReference scene)
        {
            SceneManager.UnloadSceneAsync(scene.Name);
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
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene.Name);
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

        #endregion

        #region Merge Scenes and Move GameObjects

        public static void MergeScenes(SceneReference sourceScene, SceneReference destinationScene)
        {
            SceneManager.MergeScenes(sourceScene.Scene, destinationScene.Scene);
        }

        public static void MoveGameObjectToScene(GameObject go, SceneReference scene)
        {
            SceneManager.MoveGameObjectToScene(go, scene.Scene);
        }

        #endregion

        #region Set Active Scene

        public static bool SetActiveScene(SceneReference scene)
        {
            return SceneManager.SetActiveScene(scene.Scene);
        }

        #endregion

        #endregion

        #region Scene Event Handlers

        private static void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            SceneLoaded?.Invoke(SceneReferenceFrom(scene));
            
            if (Settings.LogSceneLoaded)
                Log($"Scene {scene.name} loaded");
        }

        private static void OnSceneUnloadedHandler(Scene scene)
        {
            SceneUnloaded?.Invoke(SceneReferenceFrom(scene));

            CurrentScene = null;
            
            if (Settings.LogSceneUnloaded)
                Log($"Scene {scene.name} unloaded");
        }

        private static void OnActiveSceneChangedHandler(Scene previousScene, Scene newScene)
        {
            CurrentScene = SceneReferenceFrom(newScene);
            ActiveSceneChanged?.Invoke(SceneReferenceFrom(previousScene), CurrentScene);
            CurrentSceneChanged?.Invoke();

            if (Settings.LogSceneSwitched)
                Log($"Active scene changed from {newScene.name} to {CurrentScene.Name}");
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

        public static SceneReference SceneReferenceFromPath(string scenePath)
        {
            return new SceneReference(scenePath);
        }

        #endregion

        #region Check Scene

        public static bool IsSceneLoaded(SceneReference scene) => scene.IsLoaded();

        public static bool IsSceneLoaded(string sceneName) => SceneManager.GetSceneByName(sceneName).isLoaded;

        public static bool IsSceneLoaded(int sceneBuildIndex) =>
            SceneManager.GetSceneByBuildIndex(sceneBuildIndex).isLoaded;

        public static bool IsSceneValid(SceneReference scene) => scene.IsValid();

        public static bool IsSceneValid(string sceneName) => SceneManager.GetSceneByName(sceneName).IsValid();

        public static bool IsSceneValid(int sceneBuildIndex) =>
            SceneManager.GetSceneByBuildIndex(sceneBuildIndex).IsValid();

        public static bool IsSceneLoadedInHierarchy(SceneReference scene) => GetLoadedScenes().Contains(scene);

        #endregion

        #region Scene List

        public static void AddSceneToQueue(SceneReference scene)
        {
            SceneQueue.Add(scene);
        }

        public static void RemoveSceneFromQueue(SceneReference scene)
        {
            SceneQueue.Remove(scene);
        }

        public static void ClearSceneQueue()
        {
            SceneQueue.Clear();
        }

        /// <summary>
        /// Loads the next scene in the queue. 
        /// </summary>
        public static void LoadNextSceneInQueue()
        {
            if (SceneQueue.Count <= 0)
            {
                LogWarning("No scenes in queue");
                return;
            }

            if (IndexInSceneQueue >= SceneQueue.Count)
            {
                LogWarning("No more scenes in queue");
                return;
            }

            LoadScene(SceneQueue.Scenes[IndexInSceneQueue]);

            if (IndexInSceneQueue < SceneQueue.Count - 1)
            {
                IndexInSceneQueue++;
            }
        }

        /// <summary>
        /// Loads the next scene in the queue.
        /// </summary>
        /// <returns>The AsyncOperation loading the scene or null if there is no next scene</returns>
        public static AsyncOperation LoadNextSceneInQueueAsync()
        {
            if (SceneQueue.Count <= 0)
            {
                LogWarning("No scenes in queue");
                return null;
            }

            if (IndexInSceneQueue >= SceneQueue.Count)
            {
                LogWarning("No more scenes in queue");
                return null;
            }

            SceneReference scene = SceneQueue.Scenes[IndexInSceneQueue];
            AsyncOperation operation = LoadSceneAsync(scene);
            operation.completed += _ => { SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.Path)); };

            if (IndexInSceneQueue < SceneQueue.Count - 1)
            {
                IndexInSceneQueue++;
            }

            return operation;
        }
        
        public static SceneReference GetSceneInQueue(int index)
        {
            return SceneQueue.Scenes[index];
        }
        
        public static SceneReference GetNextSceneInQueue()
        {
            if (SceneQueue.Count <= 0)
            {
                LogWarning("No scenes in queue");
                return null;
            }

            if (IndexInSceneQueue >= SceneQueue.Count)
            {
                LogWarning("No more scenes in queue");
                return null;
            }

            return SceneQueue.Scenes[IndexInSceneQueue];
        }

        #endregion

        #region Get Scene(s)

        public static List<SceneReference> GetLoadedScenes()
        {
            int sceneCount = SceneManager.loadedSceneCount;
            List<SceneReference> scenes = new(sceneCount);
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                scenes.Add(SceneReferenceFrom(scene));
            }

            return scenes;
        }

        #endregion

        #region Set Scene

        public static bool SetActiveAndCurrentScene(SceneReference scene)
        {
            bool value = false;
            if (scene.IsValid())
            {
                value = SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.Path));
            }

            return value;
        }

        #endregion

        #region Build Settings

#if UNITY_EDITOR
        internal static int AddSceneToBuildSettings(SceneReference scene)
        {
            if (scene == null)
            {
                LogError("Scene is null");
                return -1;
            }

            if (scene.BuildIndex >= 0)
            {
                return scene.BuildIndex;
            }

            EditorBuildSettingsScene[] newScenes =
            {
                new(scene.Path, true)
            };

            if (EditorBuildSettings.scenes.ToList().Any(s => s.path == scene.Path))
            {
                return scene.BuildIndex;
            }

            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(newScenes).ToArray();

            return scene.BuildIndex;
        }
#endif

        #endregion
        
        #region Persistent Data
        
        public static void Persist<T>(string key, T data)
        {
            PersistentDataManager.Set(key, data);
        }
        
        public static T GetPersisted<T>(string key)
        {
            return PersistentDataManager.Get<T>(key);
        }
        
        public static bool TryGetPersisted<T>(string key, out T data)
        {
            return PersistentDataManager.TryGet(key, out data);
        }
        
        #endregion
    }
}