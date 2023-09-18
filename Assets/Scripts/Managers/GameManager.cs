using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(PlayerInput))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public PlayerInput PlayerInput { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                PlayerInput = GetComponent<PlayerInput>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Destroy any duplicate GameManagers
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //bool isPreloadSceneOnly = true;

            //// Loop through all loaded scenes
            //for (int i = 0; i < SceneManager.sceneCount; ++i)
            //{
            //    Scene scene = SceneManager.GetSceneAt(i);

            //    // If we find a scene that isn't the Preload scene, we set our flag to false
            //    if (scene.name != SceneNames.Preload)
            //    {
            //        isPreloadSceneOnly = false;
            //        break;
            //    }
            //}

            //// If only the Preload scene is loaded, then load the default scene
            //if (isPreloadSceneOnly)
            //{
            //    // Load the scene specified in the _sceneToLoad property
            //    // We're using LoadSceneMode.Additive to keep the current scene open
            //    SceneManager.LoadScene(SceneNames.MainGame);
            //}
        }
    }
}