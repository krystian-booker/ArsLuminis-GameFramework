using Audio;
using Cinemachine;
using Dialog;
using EventSystem;
using Input;
using Scene;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools
{
    public static class Systems
    {
        public static readonly GameManager GameManager;
        public static readonly InputManager InputManager;
        public static readonly DialogManager DialogManager;
        public static readonly SceneControlManager SceneControlManager;
        public static readonly AudioManager AudioManager;
        
        public static readonly Camera MainCamera;
        public static readonly CinemachineBrain CinemachineBrain;
        public static readonly UnityEngine.EventSystems.EventSystem EventSystem;

        static Systems()
        {
            var app = GameObject.Find("__app");

            //Create references
            GameManager = app.GetComponent<GameManager>();
            InputManager = app.GetComponent<InputManager>();
            DialogManager = app.GetComponent<DialogManager>();
            SceneControlManager = app.GetComponent<SceneControlManager>();
            AudioManager = app.GetComponent<AudioManager>();
            MainCamera = Camera.main;
            EventSystem = UnityEngine.EventSystems.EventSystem.current;

            //Validate components
            Assert.IsNotNull(GameManager, $"{nameof(Systems)}: GameManager is missing from _preload");
            Assert.IsNotNull(InputManager, $"{nameof(Systems)}: InputManager is missing from _preload");
            Assert.IsNotNull(DialogManager, $"{nameof(Systems)}: DialogManager is missing from _preload");
            Assert.IsNotNull(SceneControlManager, $"{nameof(Systems)}: SceneControlManager is missing from _preload");
            Assert.IsNotNull(AudioManager, $"{nameof(Systems)}: AudioManager is missing from _preload");
            Assert.IsNotNull(MainCamera, $"{nameof(Systems)}: Camera is missing from _preload");
            Assert.IsNotNull(EventSystem, $"{nameof(Systems)}: EventSystem is missing from _preload");
            
            CinemachineBrain = MainCamera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(CinemachineBrain, $"{nameof(Systems)}: CinemachineBrain is missing from _preload camera");
        }
    }
}