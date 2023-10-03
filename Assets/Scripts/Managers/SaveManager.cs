using Assets.Scripts.Models;
using Assets.Scripts.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SaveManager : MonoBehaviour
    {
        private const string DEBUG_MODE_KEY = "IsDebugMode";
        public static bool IsDebugMode
        {
            get => PlayerPrefs.GetInt(DEBUG_MODE_KEY, 1) == 1;
            set => PlayerPrefs.SetInt(DEBUG_MODE_KEY, value ? 1 : 0);
        }

        private static readonly object LockObject = new object();

        private static int saveCounter = 0;
        private static string autoSaveFilePath;
        private static Dictionary<string, Dictionary<string, object>> currentSave;
        private static Dictionary<string, ISaveable> saveableObjects = new Dictionary<string, ISaveable>();

        private void Start()
        {
            InitializeSaveCounter();
        }

        private void InitializeSaveCounter()
        {
            autoSaveFilePath = Path.Combine(Application.persistentDataPath, "autoSaveData");
            var saveFileDirectory = Application.persistentDataPath;
            var searchPattern = "saveData*.dat";

            var foundSaveFiles = Directory.GetFiles(saveFileDirectory, searchPattern);

            if (foundSaveFiles.Length == 0)
            {
                saveCounter = 0;
                return;
            }

            var indices = new List<int>();
            foreach (var file in foundSaveFiles)
            {
                var fileName = Path.GetFileName(file);
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

            if (foundSaveFiles.Contains(autoSaveFilePath))
            {
                foundSaveFiles.Remove(autoSaveFilePath);
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
            if (File.Exists(autoSaveFilePath))
            {
                string autoSaveFileName = Path.GetFileName(autoSaveFilePath);
                string autoSaveDateTimeStr = autoSaveFileName.Split('_').LastOrDefault()?.Replace(".dat", string.Empty);

                DateTime.TryParseExact(autoSaveDateTimeStr, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime autoSaveDateTime);

                string autoSaveScreenshotPath = Path.ChangeExtension(autoSaveFilePath, "png");

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
                filePath = autoSaveFilePath;
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
                var serializableData = ConvertToSerializableData(data);
                dataDictionary.Add(guid, serializableData);
            }
            return dataDictionary;
        }

        private static Dictionary<string, object> ConvertToSerializableData(object data)
        {
            var serializableData = new Dictionary<string, object>();
            foreach (var field in data.GetType().GetFields())
            {
                if (field.Name == "Guid") continue;
                var fieldValue = field.GetValue(data);
                var serializableValue = ConvertToSerializableValue(field, fieldValue);
                serializableData.Add(field.Name, serializableValue);
            }
            return serializableData;
        }

        private static object ConvertToSerializableValue(FieldInfo field, object value)
        {
            return field.FieldType switch
            {
                Type t when t == typeof(Vector3) => new SerializableVector3((Vector3)value),
                Type t when t == typeof(Vector3Int) => new SerializableVector3Int((Vector3Int)value),
                Type t when t == typeof(Vector2) => new SerializableVector2((Vector2)value),
                Type t when t == typeof(Vector2Int) => new SerializableVector2Int((Vector2Int)value),
                Type t when t == typeof(Vector4) => new SerializableVector4((Vector4)value),
                Type t when t == typeof(Quaternion) => new SerializableQuaternion((Quaternion)value),
                Type t when t == typeof(Matrix4x4) => new SerializableMatrix4x4((Matrix4x4)value),
                Type t when t == typeof(LayerMask) => new SerializableLayerMask((LayerMask)value),
                Type t when t == typeof(Hash128) => new SerializableHash128((Hash128)value),
                Type t when t == typeof(Color32) => new SerializableColor32((Color32)value),
                _ => value
            };
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
            // Ordering dataList by Priority before loading each object
            var orderedDataList = dataList
                                  .OrderBy(dataDict => Convert.ToInt32(dataDict.Value["Priority"]))
                                  .ToList();

            foreach (var kvp in orderedDataList)
            {
                var guid = kvp.Key;
                var dataDict = kvp.Value;

                if (saveableObjects.TryGetValue(guid, out ISaveable saveableObject))
                {
                    // Create an instance of the appropriate SaveableData type.
                    var data = saveableObject.Save();
                    ConvertAndSetFieldValues(data, dataDict);
                    saveableObject.Load(data);
                }
                else
                {
                    Debug.LogError($"Invalid GUID: {guid} or no ISaveable object associated with it.");
                }
            }
        }

        private static void ConvertAndSetFieldValues(object data, IReadOnlyDictionary<string, object> dataDict)
        {
            foreach (var field in data.GetType().GetFields())
            {
                if (dataDict.TryGetValue(field.Name, out object fieldValue))
                {
                    object convertedValue = ConvertToOriginalValue(field, fieldValue);
                    object convertedValueForField = Convert.ChangeType(convertedValue, field.FieldType);

                    field.SetValue(data, convertedValueForField);
                }
            }
        }

        private static object ConvertToOriginalValue(FieldInfo field, object value)
        {
            return field.FieldType switch
            {
                Type t when t == typeof(Vector3) && value is SerializableVector3 sVector3 => (Vector3)sVector3,
                Type t when t == typeof(Vector3Int) && value is SerializableVector3Int sVector3Int => (Vector3Int)sVector3Int,
                Type t when t == typeof(Vector2) && value is SerializableVector2 sVector2 => (Vector2)sVector2,
                Type t when t == typeof(Vector2Int) && value is SerializableVector2Int sVector2Int => (Vector2Int)sVector2Int,
                Type t when t == typeof(Vector4) && value is SerializableVector4 sVector4 => (Vector4)sVector4,
                Type t when t == typeof(Quaternion) && value is SerializableQuaternion sQuaternion => (Quaternion)sQuaternion,
                Type t when t == typeof(Matrix4x4) && value is SerializableMatrix4x4 sMatrix4x4 => (Matrix4x4)sMatrix4x4,
                Type t when t == typeof(LayerMask) && value is SerializableLayerMask sLayerMask => (LayerMask)sLayerMask,
                Type t when t == typeof(Hash128) && value is SerializableHash128 sHash128 => (Hash128)sHash128,
                Type t when t == typeof(Color32) && value is SerializableColor32 sColor32 => (Color32)sColor32,
                _ => value
            };
        }
    }
}