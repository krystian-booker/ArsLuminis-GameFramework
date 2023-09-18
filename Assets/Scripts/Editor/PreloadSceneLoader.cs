//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine.SceneManagement;
//using UnityEngine;

//[InitializeOnLoad]
//public static class PreloadSceneLoader
//{
//    static PreloadSceneLoader()
//    {
//        EditorApplication.playModeStateChanged += LoadPreloadScene;
//    }

//    static void LoadPreloadScene(PlayModeStateChange state)
//    {
//        // Check if we're about to enter Play Mode
//        if (state == PlayModeStateChange.ExitingEditMode)
//        {
//            // Check if the preload scene is already open
//            for (int i = 0; i < SceneManager.sceneCount; ++i)
//            {
//                Scene scene = SceneManager.GetSceneAt(i);
//                if (scene.name == "__preload") 
//                {
//                    return;
//                }
//            }

//            // Save the currently open scene's path
//            var currentScene = SceneManager.GetActiveScene().path;

//            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//            {
//                EditorSceneManager.OpenScene("Assets/Scenes/__preload.unity");
//                EditorApplication.delayCall += () =>
//                {
//                    EditorSceneManager.OpenScene(currentScene);
//                    EditorApplication.delayCall -= () => EditorSceneManager.OpenScene(currentScene);
//                };
//            }
//        }
//    }
//}
