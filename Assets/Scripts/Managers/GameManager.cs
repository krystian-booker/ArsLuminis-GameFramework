using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(PlayerInput))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public PlayerInput PlayerInput { get; private set; }
        public SaveManager SaveManager { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                PlayerInput = gameObject.GetComponent<PlayerInput>();
                SaveManager = gameObject.AddComponent<SaveManager>();
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}