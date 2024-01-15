using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LRS.SceneManagement.Editor
{
    public class ReliableSceneManagerEditorWindow : EditorWindow
    {
        private SceneReference _scene = new();
        
        [MenuItem("Window/LRS/Reliable Scene Manager")]
        public static void ShowWindow()
        {
            GetWindow<ReliableSceneManagerEditorWindow>("Reliable Scene Manager");
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Queue", EditorStyles.boldLabel);
            
        }
    }
}
