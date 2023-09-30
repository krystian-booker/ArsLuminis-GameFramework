using Assets.Scripts.Models;
using Assets.Scripts.Models.Interfaces;
using Newtonsoft.Json;
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
        public static bool IsDebugMode = true;


        private static readonly object LockObject = new object();
        private static SaveManager _instance;

        private static int saveCounter = 0;
        private static readonly string AutoSaveFilePath = Path.Combine(Application.persistentDataPath, "autoSaveData");
        private static Dictionary<string, Dictionary<string, object>> currentSave;

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

        public void RegisterSaveableObject(string guid, ISaveable saveableObject)
        {
            if (!saveableObjects.ContainsKey(guid))
            {
                saveableObjects.Add(guid, saveableObject);
            }
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

        public void Save(bool isAutoSave = true, int overwriteIndex = -1)
        {
            lock (LockObject)
            {
                try
                {
                    var (filePath, screenshotPath) = InitializeSavePaths(isAutoSave, overwriteIndex);
                    ScreenCapture.CaptureScreenshot(screenshotPath);

                    var dataList = PrepareSaveDataDictionary();
                    UpdateCurrentSave(dataList);

                    // Serialize object to JSON string 
                    var jsonString = JsonConvert.SerializeObject(dataList);
                    SerializeAndWriteDataToFile(filePath, jsonString);
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
                var loadPath = GetLoadPath(fileName);
                if (!File.Exists(loadPath)) throw new FileNotFoundException("No save file found!");

                try
                {
                    Dictionary<string, Dictionary<string, object>> dataList;
                    if (IsDebugMode)
                    {
                        var fileContent = File.ReadAllText(loadPath);
                        dataList = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(fileContent);
                    }
                    else
                    {
                        dataList = ReadAndDeserializeDataFromFile(loadPath);
                    }

                    if (dataList == null)
                        return;

                    currentSave = dataList ?? new Dictionary<string, Dictionary<string, object>>();
                    LoadSaveableObjectsFromDataList(dataList);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading save data: {e.Message}");
                }
            }
        }

        private (string filePath, string screenshotPath) InitializeSavePaths(bool isAutoSave, int overwriteIndex)
        {
            string filePath;
            string dateTimeStr = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (isAutoSave)
            {
                // Always overwrite the autosave file
                filePath = AutoSaveFilePath;
            }
            else if (overwriteIndex >= 0)
            {
                // Overwrite existing save file.
                filePath = Path.Combine(Application.persistentDataPath, $"saveData{overwriteIndex}_{dateTimeStr}");
            }
            else
            {
                // Create new save file.
                saveCounter++;
                filePath = Path.Combine(Application.persistentDataPath, $"saveData{saveCounter}_{dateTimeStr}");
            }

            filePath += IsDebugMode ? ".json" : ".dat";

            // Capture Screenshot
            string screenshotPath = Path.ChangeExtension(filePath, "png");

            return (filePath, screenshotPath);
        }

        private static void UpdateCurrentSave(Dictionary<string, Dictionary<string, object>> dataList)
        {
            if (currentSave == null)
            {
                currentSave = new Dictionary<string, Dictionary<string, object>>();
            }

            foreach (var kvp in dataList)
            {
                var guid = kvp.Key;
                var dataDict = kvp.Value;

                if (currentSave.ContainsKey(guid))
                {
                    // Update the existing item
                    currentSave[guid] = dataDict;
                }
                else
                {
                    // Add new item if it does not exist in currentSave
                    currentSave.Add(guid, dataDict);
                }
            }
        }

        private static Dictionary<string, Dictionary<string, object>> PrepareSaveDataDictionary()
        {
            var dataDictionary = new Dictionary<string, Dictionary<string, object>>();
            foreach (var saveableObject in saveableObjects.Values)
            {
                var guid = saveableObject.GetGuid();

                var data = saveableObject.Save();
                var serializableData = new Dictionary<string, object>();

                foreach (var field in data.GetType().GetFields())
                {
                    if (field.Name == "Guid")
                    {
                        continue;
                    }
                    else if (field.FieldType == typeof(Vector3))
                    {
                        Vector3 vector = (Vector3)field.GetValue(data);
                        serializableData.Add(field.Name, new SerializableVector3(vector));
                    }
                    else
                    {
                        serializableData.Add(field.Name, field.GetValue(data));
                    }
                }

                dataDictionary.Add(guid, serializableData);
            }

            return dataDictionary;
        }

        private void SerializeAndWriteDataToFile(string filePath, string jsonString)
        {
            if (IsDebugMode)
            {
                File.WriteAllText(filePath, jsonString);
            }
            else
            {
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
        }

        private string GetLoadPath(string fileName)
        {
            var fileExtension = IsDebugMode ? ".json" : ".dat";
            return fileName.EndsWith(fileExtension) ?
                              Path.Combine(Application.persistentDataPath, fileName) :
                              Path.Combine(Application.persistentDataPath, $"{fileName}{fileExtension}");
        }

        private Dictionary<string, Dictionary<string, object>> ReadAndDeserializeDataFromFile(string loadPath)
        {
            // Assume loadPath has been validated to exist at this point
            var lines = File.ReadAllLines(loadPath);
            if (lines.Length < 2)
            {
                Debug.LogError("Invalid save file format.");
                return new Dictionary<string, Dictionary<string, object>>();
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
                    return new Dictionary<string, Dictionary<string, object>>();
                }
            }

            // Convert hex string to JSON bytes
            var jsonBytes = Enumerable.Range(0, hexString.Length)
                                      .Where(x => x % 2 == 0)
                                      .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                                      .ToArray();

            // Deserialize JSON string to object using Newtonsoft.Json
            var jsonString = Encoding.UTF8.GetString(jsonBytes);
            return JsonUtility.FromJson<Dictionary<string, Dictionary<string, object>>>(jsonString);
        }

        private void LoadSaveableObjectsFromDataList(Dictionary<string, Dictionary<string, object>> dataList)
        {
            // Sort dataList by Priority here before loading each object
            var orderedDataList = dataList.OrderBy(dataDict => Convert.ToInt32(dataDict.Value["Priority"])).ToList();

            foreach (var kvp in orderedDataList)
            {
                var guid = kvp.Key;
                var dataDict = kvp.Value;

                if (saveableObjects.TryGetValue(guid, out ISaveable saveableObject))
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
    }
}