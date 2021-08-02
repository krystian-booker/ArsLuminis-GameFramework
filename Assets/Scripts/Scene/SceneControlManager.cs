using UnityEngine.SceneManagement;
using UnityEngine;

namespace Scene
{
    public class SceneControlManager : MonoBehaviour
    {
        public void Awake()
        {
            //Load initial scenes
            SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
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