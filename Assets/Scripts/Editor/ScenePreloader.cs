using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;
using Assets.Scripts.Constants;

namespace Assets.Scripts.Editor
{
    [InitializeOnLoad]
    public static class ScenePreloader
    {
        private const string LastSceneKey = "LastScenePath";

        static ScenePreloader()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        public static string LastScenePath
        {
            get { return SessionState.GetString(LastSceneKey, string.Empty); }
            set { SessionState.SetString(LastSceneKey, value); }
        }

        private static void PlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    // Save the current scene
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        // Remember the current scene
                        LastScenePath = EditorSceneManager.GetActiveScene().path;

                        // Load the __preload scene
                        EditorSceneManager.OpenScene($"Assets/Scenes/{SceneNames.Preload}.unity");
                    }
                    else
                    {
                        // If the user cancels the save operation, don't proceed to Play mode
                        EditorApplication.isPlaying = false;
                    }
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    if (!string.IsNullOrEmpty(LastScenePath))
                    {
                        // Load the previous scene after the __preload scene is loaded in play mode
                        EditorSceneManager.LoadSceneAsyncInPlayMode(LastScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                    }
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    // Load back the original scene after exiting play mode
                    if (!string.IsNullOrEmpty(LastScenePath))
                    {
                        EditorSceneManager.OpenScene(LastScenePath);
                    }
                    break;
            }
        }
    }
}