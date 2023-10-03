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

    [RequireComponent(typeof(PlayerInput), typeof(SaveManager))]
    public class GameManager : SaveableMonoBehaviour<GameManagerData>
    {
        public static GameManager Instance;
        public PlayerInput PlayerInput { get; private set; }
        public SaveManager SaveManager { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                PlayerInput = gameObject.GetComponent<PlayerInput>();
                SaveManager = gameObject.GetComponent<SaveManager>();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            GameManager.Instance.SaveManager.RegisterSaveableObject(this.GetGuid(), this);
        }

        public override void LoadData(GameManagerData saveData)
        {
            SceneManager.LoadScene(saveData.sceneName);
        }

        public override GameManagerData SaveData()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            var gameManagerData = new GameManagerData(this.GetGuid(), 0, currentSceneName);
            return gameManagerData;
        }
    }
}