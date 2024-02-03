using System;
using UnityEngine;

namespace LRS.SceneManagement
{
    internal class UnityMessageCallbacks : MonoBehaviour
    {
        public Action onEnable;
        public Action onAwake;
        public Action onStart;
        public Action onUpdate;
        public Action<GameObject> onDestroy;
        public Action onDisable;

        private void OnEnable() => onEnable?.Invoke();

        private void Awake() => onAwake?.Invoke();

        private void Start() => onStart?.Invoke();

        private void Update() => onUpdate?.Invoke();

        private void OnDestroy() => onDestroy?.Invoke(gameObject);

        private void OnDisable() => onDisable?.Invoke();
    }
}