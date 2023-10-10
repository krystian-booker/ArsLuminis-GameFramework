using Assets.Scripts.Constants;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Abstract;
using System;
using System.Threading.Tasks;
using UnityEditor.Build.Reporting;
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

        public override async Task LoadAsync(SaveableData data)
        {
            var saveDataTyped = (GameManagerData)data;
            await LoadSceneAsync(saveDataTyped);
        }

        private async Task LoadSceneAsync(GameManagerData saveData)
        {
            var loadSceneTask = SceneManager.LoadSceneAsync(saveData.sceneName);
            await Task.Yield();

            while (!loadSceneTask.isDone)
            {
                await Task.Delay(50);
            }

            Debug.Log("Scene load complete");
        }


        public override GameManagerData SaveData()
        {
            var activeScene = string.Empty;
            var sceneList = SceneManager.GetAllScenes();
            for (var i = 0; i < sceneList.Length; i++)
            {
                var scene = sceneList[i];
                if (scene.name != SceneNames.Preload)
                {
                    activeScene = scene.name;
                }
            }

            var gameManagerData = new GameManagerData(this.GetGuid(), 0, activeScene);
            return gameManagerData;
        }

        public override void LoadData(GameManagerData saveData)
        {
            //Skip
        }
    }
}