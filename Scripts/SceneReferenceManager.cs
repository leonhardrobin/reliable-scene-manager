using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS.SceneManagement
{
    public static class SceneReferenceManager
    {
        static SceneReferenceManager()
        {
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
            SceneManager.sceneUnloaded += OnSceneUnloadedHandler;
            SceneManager.activeSceneChanged += OnActiveSceneChangedHandler;
        }

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
        }

        private static void OnSceneUnloadedHandler(Scene scene)
        {
            SceneUnloaded?.Invoke(SceneReferenceFrom(scene));
        }

        private static void OnActiveSceneChangedHandler(Scene previousScene, Scene newScene)
        {
            ActiveSceneChanged?.Invoke(SceneReferenceFrom(previousScene), SceneReferenceFrom(newScene));
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

            if (Application.isPlaying)
            {
                Debug.LogError($"Scene \"{scene.Name}\" is not in the build settings. Cannot add scene automatically during play mode.\n" +
                         "Scenes will only be added to the build settings automatically, if not in play mode.");
                return -1;
            }
            
            Debug.Log($"Adding scene \"{scene.Name}\" to the build settings.");
            
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(newScenes).ToArray();

            return scene.BuildIndex;
        }
#endif

        #endregion
    }
}