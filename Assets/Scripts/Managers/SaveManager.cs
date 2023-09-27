using Assets.Scripts.Models;
using Assets.Scripts.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

            foreach (string file in foundSaveFiles)
            {
                string fileName = Path.GetFileName(file);
                var match = Regex.Match(fileName, @"saveData(\d+)\.dat");

                if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                {
                    saveCounter = Mathf.Max(saveCounter, index);
                }
            }
        }

        public void Save(bool isAutoSave = false, int overwriteIndex = -1)
        {
            lock (LockObject)
            {
                try
                {
                    string filePath;

                    if (isAutoSave)
                    {
                        filePath = AutoSaveFilePath; // always overwrite the autosave file
                    }
                    else if (overwriteIndex >= 0)
                    {
                        filePath = Path.Combine(Application.persistentDataPath, $"saveData{overwriteIndex}.dat"); // Overwrite existing save file.
                    }
                    else
                    {
                        filePath = Path.Combine(Application.persistentDataPath, $"saveData{++saveCounter}.dat"); // Create new save file.
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        var bf = new BinaryFormatter();
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

                        bf.Serialize(ms, dataList);
                        var hexString = BitConverter.ToString(ms.ToArray()).Replace("-", string.Empty);

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
                string loadPath = Path.Combine(Application.persistentDataPath, $"{fileName}.dat");

                if (File.Exists(loadPath))
                {
                    var bf = new BinaryFormatter();

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

                        // Convert hex string to binary and deserialize.
                        var binaryData = Enumerable.Range(0, hexString.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                            .ToArray();

                        using (MemoryStream ms = new MemoryStream(binaryData))
                        {
                            var dataList = (List<Dictionary<string, object>>)bf.Deserialize(ms);

                            foreach (var dataDict in dataList)
                            {
                                if (dataDict.TryGetValue("Guid", out object guidObj) && guidObj is string guid && saveableObjects.TryGetValue(guid, out ISaveable saveableObject))
                                {
                                    var data = saveableObject.Save(); // Create an instance of appropriate SaveableData type.

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