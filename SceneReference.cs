using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace LRS.SceneManagement
{
	/// <summary>
	/// This is based on <a href="https://github.com/NibbleByte/UnitySceneReference/blob/master/Assets/DevLocker/Utils/SceneReference.cs">NibbleByte/SceneReference</a><br/>
	/// <br/>
	/// Summary from NibbleByte/unity-scene-reference:<br/>
	/// Keeps reference to a scene asset and tracks it's path, so it can be used in the game runtime.<br/>
	/// <br/>
	/// Using <see cref="ISerializationCallbackReceiver" /> was inspired by the <a href="https://github.com/JohannesMP/unity-scene-reference">unity-scene-reference</a> implementation.<br/>
	/// This version is modified to work with the Reliable Scene Manager by <a href="https://github.com/leonhardrobin">Leonhard Robin Schnaitl</a>.<br/>
	/// <br/>
	/// Added features:<br/>
	/// <list type="bullet">
	///	<item>Added IEquatable(Scene) to check if a scene is equal to the scene reference</item>
	///	<item>Added IEquatable(SceneReference) for things like list.Contains(sceneReference)</item>
	///	<item>Implemented == and != operators</item>
	///	<item>Added IsValid() to check if the scene is valid</item>
	/// <item>Added IsLoaded() to check if the scene is loaded</item>
	/// </list>
	/// </summary>
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	[Serializable]
	public class SceneReference : ISerializationCallbackReceiver, IEquatable<SceneReference>, IEquatable<Scene>
	{
		public SceneReference() { }

		public SceneReference(string scenePath)
		{
			ScenePath = scenePath;
		}

		public SceneReference(SceneReference other)
		{
			scenePath = other.scenePath;

#if UNITY_EDITOR
			sceneAsset = other.sceneAsset;
			isDirty = other.isDirty;

			AutoUpdateReference();
#endif
		}

#if UNITY_EDITOR
		// Reference to the asset used in the editor. Player builds don't know about SceneAsset.
		// Will be used to update the scene path.
		[SerializeField] private SceneAsset sceneAsset;

#pragma warning disable 0414 // Never used warning - will be used by SerializedProperty.
		// Used to dirtify the data when needed upon displaying in the inspector.
		// Otherwise the user will never get the changes to save (unless he changes any other field of the object / scene).
		[SerializeField] private bool isDirty;
#pragma warning restore 0414
#endif

		// Player builds will use the path stored here. Should be updated in the editor or during build.
		// If scene is deleted, path will remain.
		[SerializeField] private string scenePath = string.Empty;


		/// <summary>
		/// Returns the scene path to be used in the <see cref="UnityEngine.SceneManagement.SceneManager"/> API.
		/// While in the editor, this path will always be up to date (if asset was moved or renamed).
		/// If the referred scene asset was deleted, the path will remain as is.
		/// </summary>
		public string ScenePath
		{
			get {
#if UNITY_EDITOR
				AutoUpdateReference();
#endif

				return scenePath;
			}

			set {
				scenePath = value;

#if UNITY_EDITOR
				if (string.IsNullOrEmpty(scenePath)) {
					sceneAsset = null;
					return;
				}

				sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
				if (sceneAsset == null) {
					Debug.LogError($"Setting {nameof(SceneReference)} to {value}, but no scene could be located there.");
				}
#endif
			}
		}

		/// <summary>
		/// Returns the name of the scene without the extension.
		/// </summary>
		public string SceneName => Path.GetFileNameWithoutExtension(ScenePath);

		public bool IsEmpty => string.IsNullOrEmpty(ScenePath);

		public static bool operator ==(SceneReference a, SceneReference b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
				return false;

			return a.ScenePath == b.ScenePath;
		}

		public static bool operator !=(SceneReference a, SceneReference b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is SceneReference scene)
			{
				return scenePath == scene.scenePath;
			}

			return false;
		}
		
		public bool Equals(SceneReference other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return scenePath == other.scenePath;
		}
		
		public bool Equals(Scene other)
		{
			return scenePath == other.path;
		}
		
		public override int GetHashCode()
		{
			return scenePath != null ? scenePath.GetHashCode() : 0;
		}

		/// <summary>
		///	Hand through from <see cref="Scene.IsValid"/>.
		/// </summary>
		/// <returns>Whether this is a valid Scene.</returns>
		public bool IsValid()
		{
			return !string.IsNullOrEmpty(scenePath) && SceneManager.GetSceneByPath(scenePath).IsValid();
		}
		
		/// <summary>
		/// Hand through from <see cref="Scene.isLoaded"/>.
		/// </summary>
		public bool IsLoaded()
		{
			return !string.IsNullOrEmpty(scenePath) && SceneManager.GetSceneByPath(scenePath).isLoaded;
		}

#if UNITY_EDITOR
		private static bool _reloadingAssemblies = false;

		static SceneReference()
		{
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
		}

		private static void OnBeforeAssemblyReload()
		{
			_reloadingAssemblies = true;
		}
#endif

		public SceneReference Clone() => new(this);

		public override string ToString()
		{
			return scenePath;
		}

		[Obsolete("Needed for the editor, don't use it in runtime code!", true)]
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			// In rare cases this error may be logged when trying to change SceneReference while assembly is reloading:
			// "Objects are trying to be loaded during a domain backup. This is not allowed as it will lead to undefined behaviour!"
			if (_reloadingAssemblies)
				return;

			AutoUpdateReference();
#endif
		}

		[Obsolete("Needed for the editor, don't use it in runtime code!", true)]
		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			// OnAfterDeserialize is called in the deserialization thread so we can't touch Unity API.
			// Wait for the next update frame to do it.
			EditorApplication.update += OnAfterDeserializeHandler;
#endif
		}


#if UNITY_EDITOR
		private void OnAfterDeserializeHandler()
		{
			EditorApplication.update -= OnAfterDeserializeHandler;
			AutoUpdateReference();
		}

		private void AutoUpdateReference()
		{
			if (sceneAsset == null) {
				if (string.IsNullOrEmpty(scenePath))
					return;

				SceneAsset foundAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
				if (foundAsset) {
					sceneAsset = foundAsset;
					isDirty = true;

					if (!Application.isPlaying) {
						// NOTE: This doesn't work for scriptable objects, hence the m_IsDirty.
						EditorSceneManager.MarkAllScenesDirty();
					}
				}
			} else {
				string foundPath = AssetDatabase.GetAssetPath(sceneAsset);
				if (string.IsNullOrEmpty(foundPath))
					return;

				if (foundPath != scenePath) {
					scenePath = foundPath;
					isDirty = true;

					if (!Application.isPlaying) {
						// NOTE: This doesn't work for scriptable objects, hence the m_IsDirty.
						EditorSceneManager.MarkAllScenesDirty();
					}
				}
			}
		}
#endif
	}





#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SceneReference))]
	[CanEditMultipleObjects]
	internal class SceneReferencePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty isDirtyProperty = property.FindPropertyRelative("isDirty");
			if (isDirtyProperty.boolValue) {
				isDirtyProperty.boolValue = false;
				// This will force change in the property and make it dirty.
				// After the user saves, he'll actually see the changed changes and commit them.
			}

			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			const float buildSettingsWidth = 20f;
			const float padding = 2f;

			Rect assetPos = position;
			assetPos.width -= buildSettingsWidth + padding;

			Rect buildSettingsPos = position;
			buildSettingsPos.x += position.width - buildSettingsWidth + padding;
			buildSettingsPos.width = buildSettingsWidth;

			SerializedProperty sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
			bool hadReference = sceneAssetProperty.objectReferenceValue != null;

			EditorGUI.PropertyField(assetPos, sceneAssetProperty, new GUIContent());

			int indexInSettings = -1;

			if (sceneAssetProperty.objectReferenceValue) {
				if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sceneAssetProperty.objectReferenceValue, out string guid, out long _)) {
					indexInSettings = Array.FindIndex(EditorBuildSettings.scenes, s => s.guid.ToString() == guid);
				}
			} else if (hadReference) {
				property.FindPropertyRelative("scenePath").stringValue = string.Empty;
			}

			GUIContent settingsContent = indexInSettings != -1
				? new GUIContent("-", "Scene is already in the Editor Build Settings. Click here to remove it.")
				: new GUIContent("+", "Scene is missing in the Editor Build Settings. Click here to add it.")
				;

			Color prevBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = indexInSettings != -1 ? Color.red : Color.green;

			if (GUI.Button(buildSettingsPos, settingsContent, EditorStyles.miniButtonRight) && sceneAssetProperty.objectReferenceValue) {
				if (indexInSettings != -1) {
					List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
					scenes.RemoveAt(indexInSettings);

					EditorBuildSettings.scenes = scenes.ToArray();

				} else {
					EditorBuildSettingsScene[] newScenes = {
						new (AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue), true)
					};

					EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(newScenes).ToArray();
				}
			}

			GUI.backgroundColor = prevBackgroundColor;

			EditorGUI.EndProperty();
		}
	}
#endif

}