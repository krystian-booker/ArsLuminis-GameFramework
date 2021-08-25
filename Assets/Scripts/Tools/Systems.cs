using Audio;
using Cinemachine;
using Dialog;
using Input;
using Saving;
using Scene;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools
{
    public class Systems : MonoBehaviour
    {
        #region Singleton

        public static Systems Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
        }

        #endregion

        [HideInInspector] public GameManager gameManager;
        [HideInInspector] public InputManager inputManager;
        [HideInInspector] public DialogManager dialogManager;
        [HideInInspector] public SceneControlManager sceneControlManager;
        [HideInInspector] public AudioManager audioManager;
        [HideInInspector] public SaveManager saveManager;

        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public CinemachineBrain cinemachineBrain;
        [HideInInspector] public UnityEngine.EventSystems.EventSystem eventSystem;

        private void Initialize()
        {
            var app = GameObject.Find("__app");

            //Create references
            gameManager = app.GetComponent<GameManager>();
            inputManager = app.GetComponent<InputManager>();
            dialogManager = app.GetComponent<DialogManager>();
            sceneControlManager = app.GetComponent<SceneControlManager>();
            audioManager = app.GetComponent<AudioManager>();
            saveManager = app.GetComponent<SaveManager>();
            mainCamera = Camera.main;

            //Validate components
            Assert.IsNotNull(gameManager, $"{nameof(Systems)}: GameManager is missing from _preload");
            Assert.IsNotNull(inputManager, $"{nameof(Systems)}: InputManager is missing from _preload");
            Assert.IsNotNull(dialogManager, $"{nameof(Systems)}: DialogManager is missing from _preload");
            Assert.IsNotNull(sceneControlManager, $"{nameof(Systems)}: SceneControlManager is missing from _preload");
            Assert.IsNotNull(audioManager, $"{nameof(Systems)}: AudioManager is missing from _preload");
            Assert.IsNotNull(saveManager, $"{nameof(Systems)}: SaveManager is missing from _preload");
            Assert.IsNotNull(mainCamera, $"{nameof(Systems)}: Camera is missing from _preload");

            // if (Application.isPlaying)
            // {
            //Systems is used by the Editor, these are only available when the game is running
            eventSystem = UnityEngine.EventSystems.EventSystem.current;
            Assert.IsNotNull(eventSystem, $"{nameof(Systems)}: EventSystem is missing from _preload");

            cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(cinemachineBrain, $"{nameof(Systems)}: CinemachineBrain is missing from _preload camera");
            // }
        }
    }
}