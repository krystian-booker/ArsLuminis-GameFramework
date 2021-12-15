using Audio;
using Cinemachine;
using Dialog;
using Saving;
using Scene;
using SystemInput;
using UnityEngine;
using UnityEngine.Assertions;
using UserInput;

namespace Tools
{
    public static class Systems
    {
        public static GameManager gameManager;
        public static InputManager inputManager;
        public static DialogManager dialogManager;
        public static SceneControlManager sceneControlManager;
        public static AudioManager audioManager;
        public static SaveManager saveManager;

        public static Camera mainCamera;
        public static CinemachineBrain cinemachineBrain;
        public static UnityEngine.EventSystems.EventSystem eventSystem;

        public const bool DebugWarnings = true;

        /// <summary>
        /// Only used inEditor, references are needed for scene editor tools
        /// </summary>
        static Systems()
        {
            if(!Application.isPlaying)
                return;
            
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

            if (Application.isPlaying)
            {
                //Systems is used by the Editor, these are only available when the game is running
                eventSystem = UnityEngine.EventSystems.EventSystem.current;
                Assert.IsNotNull(eventSystem, $"{nameof(Systems)}: EventSystem is missing from _preload");

                cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
                Assert.IsNotNull(cinemachineBrain, $"{nameof(Systems)}: CinemachineBrain is missing from _preload camera");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            var app = GameObject.Find("__app");
            
            //Create references
            gameManager = app.GetComponent<GameManager>();
            Assert.IsNotNull(gameManager, $"{nameof(Systems)}: GameManager is missing from _preload");

            inputManager = app.GetComponent<InputManager>();
            Assert.IsNotNull(inputManager, $"{nameof(Systems)}: InputManager is missing from _preload");

            dialogManager = app.GetComponent<DialogManager>();
            Assert.IsNotNull(dialogManager, $"{nameof(Systems)}: DialogManager is missing from _preload");

            sceneControlManager = app.GetComponent<SceneControlManager>();
            Assert.IsNotNull(sceneControlManager, $"{nameof(Systems)}: SceneControlManager is missing from _preload");

            audioManager = app.GetComponent<AudioManager>();
            Assert.IsNotNull(audioManager, $"{nameof(Systems)}: AudioManager is missing from _preload");

            saveManager = app.GetComponent<SaveManager>();
            Assert.IsNotNull(saveManager, $"{nameof(Systems)}: SaveManager is missing from _preload");

            eventSystem = UnityEngine.EventSystems.EventSystem.current;
            Assert.IsNotNull(eventSystem, $"{nameof(Systems)}: EventSystem is missing from _preload");

            mainCamera = Camera.main;
            Assert.IsNotNull(mainCamera, $"{nameof(Systems)}: Camera is missing from _preload");

            cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(cinemachineBrain, $"{nameof(Systems)}: CinemachineBrain is missing from _preload camera");
        }
    }
}