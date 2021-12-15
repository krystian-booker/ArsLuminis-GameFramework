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
            //We don't want to load a scene if there is one already open in the editor
            if (SceneManager.sceneCount == 1)
            {
                if (overrideScene != null && Application.isEditor)
                {
                    SceneManager.LoadSceneAsync(overrideScene.SceneName, LoadSceneMode.Additive);
                }
                else if (initialScene != null)
                {
                    SceneManager.LoadSceneAsync(initialScene.SceneName, LoadSceneMode.Additive);
                }
            }

            //UI
            if (loadUI)
            {
                SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            }
        }

        public void SwitchScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAdditive(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}