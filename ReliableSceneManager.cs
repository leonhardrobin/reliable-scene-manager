using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS.SceneManagement
{
    public class ReliableSceneManager : SceneManagerAPI
    {
        public static SceneReference CurrentScene { get; private set; }
        
        #region Basic Scene Loading
        
        public static void LoadScene(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(scene.ScenePath, mode);
            CurrentScene = scene;
        }
        
        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, mode);
            CurrentScene = SceneReferenceFrom(sceneName);
        }
        
        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneBuildIndex, mode);
            CurrentScene = SceneReferenceFrom(sceneBuildIndex);
        }
        
        public static void LoadScene(SceneReference scene, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(scene.ScenePath, parameters);
            CurrentScene = scene;
        }
        
        public static void LoadScene(string sceneName, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneName, parameters);
            CurrentScene = SceneReferenceFrom(sceneName);
        }
        
        public static void LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneBuildIndex, parameters);
            CurrentScene = SceneReferenceFrom(sceneBuildIndex);
        }
        
        public static AsyncOperation LoadSceneAsync(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ScenePath, mode);
            switch (mode)
            {
                case LoadSceneMode.Single:
                    asyncOperation.completed += _ => CurrentScene = scene;
                    break;
                case LoadSceneMode.Additive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            switch (mode)
            {
                case LoadSceneMode.Single:
                    asyncOperation.completed += _ => CurrentScene = SceneReferenceFrom(sceneName);
                    break;
                case LoadSceneMode.Additive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            switch (mode)
            {
                case LoadSceneMode.Single:
                    asyncOperation.completed += _ => CurrentScene = SceneReferenceFrom(sceneBuildIndex);
                    break;
                case LoadSceneMode.Additive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(SceneReference scene, LoadSceneParameters parameters)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ScenePath, parameters);
            switch (parameters.loadSceneMode)
            {
                case LoadSceneMode.Single:
                    asyncOperation.completed += _ => CurrentScene = scene;
                    break;
                case LoadSceneMode.Additive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameters.loadSceneMode), parameters.loadSceneMode, null);
            }
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneParameters parameters)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, parameters);
            switch (parameters.loadSceneMode)
            {
                case LoadSceneMode.Single:
                    asyncOperation.completed += _ => CurrentScene = SceneReferenceFrom(sceneName);
                    break;
                case LoadSceneMode.Additive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameters.loadSceneMode), parameters.loadSceneMode, null);
            }
            return asyncOperation;
        }
        
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneParameters parameters)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, parameters);
            switch (parameters.loadSceneMode)
            {
                case LoadSceneMode.Single:
                    asyncOperation.completed += _ => CurrentScene = SceneReferenceFrom(sceneBuildIndex);
                    break;
                case LoadSceneMode.Additive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameters.loadSceneMode), parameters.loadSceneMode, null);
            }
            return asyncOperation;
        }
        
        #endregion
        
        #region Basic Scene Unloading
        
        public static void UnloadScene(SceneReference scene)
        {
            SceneManager.UnloadSceneAsync(scene.ScenePath);
            if (CurrentScene == scene)
            {
                CurrentScene = null;
            }
        }
        
        public static void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            if (CurrentScene == SceneReferenceFrom(sceneName))
            {
                CurrentScene = null;
            }
        }
        
        public static void UnloadScene(int sceneBuildIndex)
        {
            SceneManager.UnloadSceneAsync(sceneBuildIndex);
            if (CurrentScene == SceneReferenceFrom(sceneBuildIndex))
            {
                CurrentScene = null;
            }
        }
        
        public static AsyncOperation UnloadSceneAsync(SceneReference scene)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene.ScenePath);
            if (CurrentScene == scene)
            {
                CurrentScene = null;
            }
            return asyncOperation;
        }
        
        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            if (CurrentScene == SceneReferenceFrom(sceneName))
            {
                asyncOperation.completed += _ => CurrentScene = null;
            }
            return asyncOperation;
        }
        
        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex);
            if (CurrentScene == SceneReferenceFrom(sceneBuildIndex))
            {
                asyncOperation.completed += _ => CurrentScene = null;
            }
            return asyncOperation;
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
    }
}
