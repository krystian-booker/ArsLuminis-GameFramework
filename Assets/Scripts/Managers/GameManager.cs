using Assets.Scripts.Models;
using Assets.Scripts.Models.Abstract;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    [Serializable]
    public class GameManagerData : SaveableData
    {
        public string sceneName;

        public GameManagerData(string guid, int priority, string currentScene) : base(guid, priority)
        {
            this.sceneName = currentScene;
        }
    }

    [RequireComponent(typeof(PlayerInput))]
    public class GameManager : SaveableMonoBehaviour<GameManagerData>
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

        public override void LoadData(GameManagerData saveData)
        {
            SceneManager.LoadScene(saveData.sceneName);
        }

        public override GameManagerData SaveData()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            var gameManagerData = new GameManagerData(Guid, 0, currentSceneName);
            return gameManagerData;
        }
    }
}