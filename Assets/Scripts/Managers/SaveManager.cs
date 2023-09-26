using Assets.Scripts.Models.Abstract;
using Assets.Scripts.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SaveManager : MonoBehaviour
    {
        private string SavePath => Application.persistentDataPath + "/saves/";
        private const string SaveFileExtension = ".save";
        private const string ScreenshotFileExtension = ".png";

        private List<SaveableMonoBehaviour<object>> allInstances = new List<SaveableMonoBehaviour<object>>();

        public void Register(SaveableMonoBehaviour<object> instance)
        {
            Debug.Log(instance);

            if (instance != null && !allInstances.Contains(instance))
                allInstances.Add(instance);
        }

        public void Unregister(SaveableMonoBehaviour<object> instance)
        {
            allInstances.Remove(instance);
        }

        public void SaveGame()
        {
            // Take a screenshot
            string screenshotName = "Screenshot_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ScreenshotFileExtension;
            string screenshotPath = SavePath + screenshotName;
            ScreenCapture.CaptureScreenshot(screenshotPath);

            Dictionary<string, object> saveData = new Dictionary<string, object>();
            foreach (var saveable in allInstances)
            {
                saveData[saveable.UniqueId] = saveable.Save();
            }

            // Adding DateTime and screenshot name to save data
            saveData["SaveDateTime"] = DateTime.Now;
            saveData["ScreenshotName"] = screenshotName;

            // Creating a Binary Formatter
            BinaryFormatter bf = new BinaryFormatter();

            // Create Directory if it doesn't exist
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            string fileName = "Save_" + DateTime.Now.ToString("yyyyMMddHHmmss") + SaveFileExtension;
            string path = SavePath + fileName;

            // Create and Open the file
            using (FileStream file = File.Create(path))
            {
                bf.Serialize(file, saveData);
            }
        }

        public void LoadGame(string fileName)
        {
            // Gather all ISaveable objects in the scene at the time of loading
            var _saveables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISaveable<object>>()
                .ToList();

            string path = SavePath + fileName;

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Open(path, FileMode.Open))
                {
                    Dictionary<string, object> saveData = (Dictionary<string, object>)bf.Deserialize(file);
                    if (saveData != null)
                    {
                        foreach (var saveable in _saveables)
                        {
                            if (saveData.ContainsKey(saveable.UniqueId))
                            {
                                saveable.Load(saveData[saveable.UniqueId]);
                            }
                        }
                        DateTime savedDateTime = (DateTime)saveData["SaveDateTime"];
                    }
                }
            }
            else
            {
                Debug.LogError("Save file not found.");
            }
        }

        public List<string> GetSaveFiles()
        {
            // Create Directory if it doesn't exist
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            // Fetch all the *.save files in the save directory
            return Directory.GetFiles(SavePath, "*" + SaveFileExtension).ToList();
        }
    }
}