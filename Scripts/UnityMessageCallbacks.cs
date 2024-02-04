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
        public Action onDestroy;
        public Action onDisable;

        private void OnEnable() => onEnable?.Invoke();

        private void Awake() => onAwake?.Invoke();

        private void Start() => onStart?.Invoke();

        private void Update() => onUpdate?.Invoke();

        private void OnDestroy() => onDestroy?.Invoke();

        private void OnDisable() => onDisable?.Invoke();
    }
}