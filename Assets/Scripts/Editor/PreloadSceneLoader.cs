using Assets.Scripts.Constants;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

namespace Assets.Scripts.Editor
{
    [InitializeOnLoad]
    public static class PreloadSceneLoader
    {
        static PreloadSceneLoader()
        {
            EditorApplication.playModeStateChanged += LoadPreloadScene;
        }

        static void LoadPreloadScene(PlayModeStateChange state)
        {
            // Check if we're about to enter Play Mode
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                // Check if the preload scene is already open
                for (int i = 0; i < SceneManager.sceneCount; ++i)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.name == SceneNames.Preload)
                    {
                        // Preload scene is already open, do nothing
                        return;
                    }
                }

                string currentScene = SceneManager.GetActiveScene().path;
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(string.Format("Assets/Scenes/{0}.unity", SceneNames.Preload));
                }

                // Additive load of the original scene
                EditorSceneManager.OpenScene(currentScene, OpenSceneMode.Additive);
            }
        }
    }
}
