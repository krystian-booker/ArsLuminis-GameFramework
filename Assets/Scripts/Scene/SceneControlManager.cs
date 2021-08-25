using System.Collections.Generic;
using System.Linq;
using Tools.Design;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Scene
{
    public class SceneControlManager : MonoBehaviour
    {
        [Tooltip("The initial scene the game will load after the _preload scene")]
        public SceneField initialScene;

        [Tooltip("If set this scene will be loaded instead of the initial scene. Editor use only.")]
        public SceneField overrideScene;

        [Tooltip("Should 'UI' scene be loaded in by default")]
        public bool loadUI = true;

        public void Awake()
        {
            StartPreloadScene();
        }

        public void SwitchScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAdditive(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        private void StartPreloadScene()
        {
            var openScenes = new List<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                openScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            if (openScenes.Contains("_preload") && openScenes.Count == 1)
            {
                LoadDefaultScenes();
            }
            else if (openScenes.Contains("_preload") && !openScenes.Contains("UI"))
            {
                if (loadUI)
                {
                    SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
                }
            }

        }

        private void LoadDefaultScenes()
        {
            if (overrideScene != null && Application.isEditor)
            {
                SceneManager.LoadSceneAsync(overrideScene.SceneName, LoadSceneMode.Additive);
            }
            else if (initialScene != null)
            {
                SceneManager.LoadSceneAsync(initialScene.SceneName, LoadSceneMode.Additive);
            }

            if (loadUI)
            {
                SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            }
        }
    }
}