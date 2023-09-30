using Assets.Scripts.Models;
using Assets.Scripts.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SaveManager : MonoBehaviour
    {
        private static readonly object LockObject = new object();
        private static SaveManager _instance;

        private static int saveCounter = 0;
        private static readonly string AutoSaveFilePath = Path.Combine(Application.persistentDataPath, "autoSaveData.dat");

        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to find existing instance
                    _instance = FindObjectOfType<SaveManager>();

                    // If no instance was found, create a new one
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("SaveManager");
                        _instance = go.AddComponent<SaveManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private static Dictionary<string, ISaveable> saveableObjects = new Dictionary<string, ISaveable>();
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            InitializeSaveCounter();
        }

        private void InitializeSaveCounter()
        {
            string saveFileDirectory = Application.persistentDataPath;
            string searchPattern = "saveData*.dat";

            string[] foundSaveFiles = Directory.GetFiles(saveFileDirectory, searchPattern);

            if (foundSaveFiles.Length == 0)
            {
                saveCounter = 0;
                return;
            }

            List<int> indices = new List<int>();
            foreach (string file in foundSaveFiles)
            {
                string fileName = Path.GetFileName(file);
                var match = Regex.Match(fileName, @"saveData(\d+)\.dat");

                if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                {
                    indices.Add(index);
                }
            }

            // Set saveCounter to the maximum index found, or to 0 if no valid files were found.
            saveCounter = indices.Any() ? indices.Max() : 0;
        }

        public List<SaveData> GetAllSaves()
        {
            string saveFileDirectory = Application.persistentDataPath;
            string searchPattern = "saveData*.dat";

            List<string> foundSaveFiles = Directory.GetFiles(saveFileDirectory, searchPattern).ToList();

            List<SaveData> saveDataModels = new List<SaveData>();

            if (foundSaveFiles.Contains(AutoSaveFilePath))
            {
                foundSaveFiles.Remove(AutoSaveFilePath);
            }

            // Loop through and create SaveDataModel objects for each save file.
            foreach (var saveFile in foundSaveFiles)
            {
                var fileName = Path.GetFileName(saveFile);
                var match = Regex.Match(fileName, @"saveData(\d+)_(\d+)\.dat");

                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out int index);
                    DateTime.TryParseExact(match.Groups[2].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);

                    string screenshotPath = Path.ChangeExtension(saveFile, "png");

                    saveDataModels.Add(new SaveData { Index = index, DateTime = dateTime, ScreenshotPath = screenshotPath, FileName = fileName });
                }
            }

            // Sort the models list by DateTime in descending order.
            saveDataModels = saveDataModels.OrderByDescending(model => model.DateTime).ToList();

            // If autoSave exists, add it to the front of the list.
            if (File.Exists(AutoSaveFilePath))
            {
                string autoSaveFileName = Path.GetFileName(AutoSaveFilePath);
                string autoSaveDateTimeStr = autoSaveFileName.Split('_').LastOrDefault()?.Replace(".dat", string.Empty);

                DateTime.TryParseExact(autoSaveDateTimeStr, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime autoSaveDateTime);

                string autoSaveScreenshotPath = Path.ChangeExtension(AutoSaveFilePath, "png");

                SaveData autoSaveDataModel = new SaveData
                {
                    Index = -1,
                    DateTime = autoSaveDateTime,
                    ScreenshotPath = autoSaveScreenshotPath,
                    FileName = autoSaveFileName
                };

                saveDataModels.Insert(0, autoSaveDataModel);
            }

            return saveDataModels;
        }

        public void Save(bool isAutoSave = false, int overwriteIndex = -1)
        {
            lock (LockObject)
            {
                try
                {
                    string filePath;
                    string dateTimeStr = DateTime.Now.ToString("yyyyMMddHHmmss");

                    if (isAutoSave)
                    {
                        filePath = AutoSaveFilePath; // always overwrite the autosave file
                    }
                    else if (overwriteIndex >= 0)
                    {
                        filePath = Path.Combine(Application.persistentDataPath, $"saveData{overwriteIndex}_{dateTimeStr}.dat"); // Overwrite existing save file.
                    }
                    else
                    {
                        saveCounter++;
                        filePath = Path.Combine(Application.persistentDataPath, $"saveData{saveCounter}_{dateTimeStr}.dat"); // Create new save file.
                    }

                    // Capture Screenshot
                    string screenshotPath = Path.ChangeExtension(filePath, "png");
                    ScreenCapture.CaptureScreenshot(screenshotPath);

                    var dataList = new List<Dictionary<string, object>>();
                    foreach (var saveableObject in saveableObjects.Values)
                    {
                        var data = saveableObject.Save();
                        var serializableData = new Dictionary<string, object>();

                        foreach (var field in data.GetType().GetFields())
                        {
                            if (field.FieldType == typeof(Vector3))
                            {
                                Vector3 vector = (Vector3)field.GetValue(data);
                                serializableData.Add(field.Name, new SerializableVector3(vector));
                            }
                            else
                            {
                                serializableData.Add(field.Name, field.GetValue(data));
                            }
                        }

                        dataList.Add(serializableData);
                    }

                    // Serialize object to JSON string 
                    string jsonString = JsonUtility.ToJson(dataList);

                    // Convert JSON string to hex
                    var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                    var hexString = BitConverter.ToString(jsonBytes).Replace("-", string.Empty);

                    // Compute the checksum of the hex string.
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(hexString));
                        string checksum = BitConverter.ToString(bytes).Replace("-", string.Empty);

                        // Save the checksum along with the hex string to the file.
                        File.WriteAllText(filePath, $"{checksum}\n{hexString}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error occurred while saving data: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public void Load(string fileName = "autoSaveData")
        {
            lock (LockObject)
            {
                string loadPath = fileName.EndsWith(".dat") ?
                                  Path.Combine(Application.persistentDataPath, fileName) :
                                  Path.Combine(Application.persistentDataPath, $"{fileName}.dat");

                if (File.Exists(loadPath))
                {
                    try
                    {
                        // Read the checksum and hex string from the file.
                        var lines = File.ReadAllLines(loadPath);
                        if (lines.Length < 2)
                        {
                            Debug.LogError("Invalid save file format.");
                            return;
                        }

                        var readChecksum = lines[0];
                        var hexString = lines[1];

                        // Compute the checksum of the hex string and compare.
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(hexString));
                            var computedChecksum = BitConverter.ToString(bytes).Replace("-", string.Empty);

                            if (computedChecksum != readChecksum)
                            {
                                Debug.LogError("Checksum validation failed. The save file might be corrupted.");
                                return;
                            }
                        }

                        // Convert hex string to JSON bytes
                        var jsonBytes = Enumerable.Range(0, hexString.Length)
                                                  .Where(x => x % 2 == 0)
                                                  .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                                                  .ToArray();

                        // Deserialize JSON string to object using Newtonsoft.Json
                        var jsonString = Encoding.UTF8.GetString(jsonBytes);

                        var dataList = JsonUtility.FromJson<List<Dictionary<string, object>>>(jsonString);

                        foreach (var dataDict in dataList)
                        {
                            if (dataDict.TryGetValue("Guid", out object guidObj) && guidObj is string guid && saveableObjects.TryGetValue(guid, out ISaveable saveableObject))
                            {
                                // Create an instance of appropriate SaveableData type.
                                var data = saveableObject.Save();

                                foreach (var field in data.GetType().GetFields())
                                {
                                    if (dataDict.TryGetValue(field.Name, out object fieldValue))
                                    {
                                        if (field.FieldType == typeof(Vector3) && fieldValue is SerializableVector3 serializableVector)
                                        {
                                            field.SetValue(data, (Vector3)serializableVector);
                                        }
                                        else
                                        {
                                            field.SetValue(data, fieldValue);
                                        }
                                    }
                                }

                                saveableObject.Load(data);
                            }
                            else
                            {
                                Debug.LogError("Invalid GUID or no ISaveable object associated with it.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error loading save data: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError("No save file found!");
                }
            }
        }

        public void RegisterSaveableObject(string guid, ISaveable saveableObject)
        {
            if (!saveableObjects.ContainsKey(guid))
            {
                saveableObjects.Add(guid, saveableObject);
            }
        }
    }
}