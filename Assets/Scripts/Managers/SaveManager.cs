using Assets.Scripts.Models;
using Assets.Scripts.Models.Interfaces;
using Assets.Scripts.Models.Serializables;
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
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SaveManager : MonoBehaviour
    {
        // Constants
        private const string DEBUG_MODE_KEY = "IsDebugMode";

        // Static fields
        private static int saveCounter = 0;
        private static string autoSaveFilePath;
        private static Dictionary<string, Dictionary<string, object>> currentSave;
        private static Dictionary<string, ISaveable> saveableObjects = new Dictionary<string, ISaveable>();

        // Instance fields
        private SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);

        // Properties
        public static bool IsDebugMode
        {
            get => PlayerPrefs.GetInt(DEBUG_MODE_KEY, 1) == 1;
            set => PlayerPrefs.SetInt(DEBUG_MODE_KEY, value ? 1 : 0);
        }

        // Lifecycle methods
        private void Start()
        {
            InitializeSaveCounter();
        }

        // Public methods
        public void RegisterSaveableObject(string guid, ISaveable saveableObject)
        {
            if (!saveableObjects.ContainsKey(guid))
            {
                saveableObjects.Add(guid, saveableObject);
            }
        }

        /// <summary>
        /// Retrieves a list of all save data present in the application's persistent data path.
        /// </summary>
        /// <returns>A list of SaveData objects representing each save, sorted by the save date and time in descending order. The auto-save, if present, is added to the front of the list.</returns>
        /// <remarks>
        /// This method searches the application's persistent data path for save files matching the pattern "saveData*.dat".
        /// For each valid save file found, it extracts metadata such as the index, date, time, and corresponding screenshot path.
        /// The method also checks for an auto-save file and, if found, constructs a SaveData object and adds it to the front of the return list.
        /// Invalid or mismatched save files are ignored during the process.
        /// </remarks>
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

        /// <summary>
        /// Asynchronously saves game data to a specified save file or the default auto-save file, capturing a screenshot as well.
        /// </summary>
        /// <param name="isAutoSave">Indicates whether to save to the automatic save file. Defaults to true.</param>
        /// <param name="overwriteIndex">If non-negative, specifies the index of an existing save file to overwrite. If negative, the auto-save file is used. Defaults to -1.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        /// <remarks>
        /// The method initiates the path to the save file and corresponding screenshot path based on the provided parameters. It captures a screenshot of the current game state.
        /// Next, it prepares the save data dictionary, updates the current save data, and then serializes this data into a JSON string.
        /// The serialized string is then written to the specified save file.
        /// If any exceptions arise during the save process, they are logged.
        /// A semaphore lock is used to ensure thread-safety during the save process.
        /// </remarks>
        public async Task Save(bool isAutoSave = true, int overwriteIndex = -1)
        {
            await _lockSemaphore.WaitAsync();
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
            finally
            {
                _lockSemaphore.Release();
            }
        }

        /// <summary>
        /// Asynchronously loads game data from the specified save file or the default auto-save file.
        /// </summary>
        /// <param name="isAutoSave">Indicates whether to load from the automatic save file. Defaults to true.</param>
        /// <param name="overwriteIndex">If non-negative, specifies the index of an existing save file to load from. If negative, the auto-save file is used. Defaults to -1.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        /// <remarks>
        /// The method initializes the path to the save file based on the provided parameters. It checks if the save file exists and then:
        /// - If in debug mode, reads the JSON content directly.
        /// - Otherwise, deserializes the data from the file format.
        /// After obtaining the save data list, the current save data is updated, and the saveable objects are loaded from the data list.
        /// If any exceptions are encountered during the process, they are logged and rethrown.
        /// A semaphore lock ensures thread-safety during the loading process.
        /// </remarks>
        public async Task Load(bool isAutoSave = true, int overwriteIndex = -1)
        {
            await _lockSemaphore.WaitAsync();

            try
            {
                var (filePath, _) = InitializeSavePaths(isAutoSave, overwriteIndex);
                if (!File.Exists(filePath)) throw new FileNotFoundException("No save file found!");

                Dictionary<string, Dictionary<string, object>> dataList;
                if (IsDebugMode)
                {
                    var fileContent = File.ReadAllText(filePath);
                    dataList = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(fileContent);
                }
                else
                {
                    dataList = ReadAndDeserializeDataFromFile(filePath);
                }

                if (dataList == null)
                    return;

                currentSave = dataList;
                await LoadSaveableObjectsFromDataList(dataList);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }

        #region Private methods

        /// <summary>
        /// Initializes the save counter based on the existing save files in the application's persistent data path.
        /// </summary>
        /// <remarks>
        /// The method sets the path for the auto-save file and then checks the application's persistent data path for existing save files that match the pattern "saveData*.dat".
        /// For each valid save file found, it extracts the numeric index from the file name using regular expressions.
        /// The save counter is then set to the maximum index found among the valid save files or to 0 if no valid save files are detected.
        /// </remarks>
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

        /// <summary>
        /// Initializes and returns the file paths for saving game data and a corresponding screenshot.
        /// </summary>
        /// <param name="isAutoSave">Indicates whether the save operation is an automatic save.</param>
        /// <param name="overwriteIndex">If non-negative, specifies the index of an existing save file to overwrite. If negative, a new save file is created.</param>
        /// <returns>
        /// A tuple containing two string paths:
        /// - The first path (filePath) is where the game data will be saved.
        /// - The second path (screenshotPath) is where the game's screenshot will be saved.
        /// </returns>
        /// <remarks>
        /// Depending on the method parameters and whether the application is in debug mode:
        /// - In AutoSave mode, the autosave file is always overwritten.
        /// - If overwriteIndex is provided and non-negative, an existing save file is overwritten with a timestamp.
        /// - Otherwise, a new save file is created with an incremented counter and timestamp.
        /// The file extensions will be ".json" for debug mode and ".dat" otherwise. A screenshot is also captured with the ".png" extension.
        /// </remarks>
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

        /// <summary>
        /// Updates the current save data with the provided data list, either by updating existing entries or adding new ones.
        /// </summary>
        /// <param name="dataList">A dictionary containing the save data to be updated or added to the current save data.</param>
        /// <remarks>
        /// This method checks each entry in the provided dataList:
        /// - If an entry with the same GUID already exists in the current save data, it is updated with the new data.
        /// - If no matching entry exists in the current save data, a new entry is added.
        /// If the current save data is uninitialized (null), it is initialized with a new empty dictionary before proceeding with the update.
        /// </remarks>
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

        /// <summary>
        /// Prepares and returns a dictionary containing serialized data for all saveable objects.
        /// </summary>
        /// <returns>
        /// A dictionary where each key is the GUID of a saveable object and each value is a nested dictionary representing the serializable data of the object's fields.
        /// </returns>
        /// <remarks>
        /// This method iterates over each saveable object, retrieves its GUID, and calls its Save() method to get its data. 
        /// The data is then converted to a serializable format using the ConvertToSerializableData method and added to the return dictionary using the GUID as the key.
        /// </remarks>
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

        /// <summary>
        /// Converts an object's fields to a dictionary with serializable values, excluding the "Guid" field.
        /// </summary>
        /// <param name="data">The object whose fields are to be converted to serializable values.</param>
        /// <returns>A dictionary where each key is a field name and each value is its corresponding serializable representation.</returns>
        /// <remarks>
        /// This method iterates over each field of the provided object and converts its value to a serializable format using the ConvertToSerializableValue method.
        /// The "Guid" field is explicitly skipped during this conversion process.
        /// </remarks>
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

        /// <summary>
        /// Converts the given value to a serializable format based on the field's type.
        /// </summary>
        /// <param name="field">The FieldInfo representing the field whose value is being converted.</param>
        /// <param name="value">The original value of the field that needs conversion.</param>
        /// <returns>
        /// A serializable representation of the value. If the field type matches specific types (e.g., Vector3, Vector2Int), 
        /// it returns an instance of the corresponding serializable class (e.g., SerializableVector3, SerializableVector2Int). 
        /// For other types, the original value is returned.
        /// </returns>
        /// <remarks>
        /// This method is designed to handle fields of specific Unity types (like Vector3, Quaternion, etc.) 
        /// that may not be directly serializable in some serialization frameworks. By converting these types to 
        /// their serializable counterparts, it facilitates their serialization and storage.
        /// </remarks>
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

        /// <summary>
        /// Serializes and writes data in JSON format to a file. If not in debug mode, the data is saved as a hex string with a checksum for validation.
        /// </summary>
        /// <param name="filePath">The path to the file where the serialized data will be written.</param>
        /// <param name="jsonString">The JSON-formatted string containing the data to be written to the file.</param>
        /// <remarks>
        /// In debug mode, the data is written directly in JSON format to the file.
        /// In non-debug mode:
        /// 1. The JSON string is converted to a hexadecimal string.
        /// 2. A checksum of the hex string is computed using SHA256.
        /// 3. The checksum and the hex string are written to the file, with the checksum on the first line and the hex string on the second line.
        /// This structure helps ensure the integrity of the saved data when read back from the file.
        /// </remarks>
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

        /// <summary>
        /// Reads and deserializes save data from a file located at the specified path.
        /// </summary>
        /// <param name="loadPath">The path to the file containing the save data to read and deserialize.</param>
        /// <returns>A dictionary mapping GUID strings to their associated data dictionary. If the file format is invalid or checksum validation fails, an empty dictionary is returned.</returns>
        /// <remarks>
        /// The file at the given load path is expected to have the following format:
        /// 1. The first line contains a checksum of the data.
        /// 2. The second line contains the serialized data in hexadecimal format.
        /// The method computes the checksum of the hex string and compares it to the read checksum to ensure data integrity.
        /// If the checksum validation is successful, the hex string is converted to a JSON byte array, which is then deserialized into the return dictionary.
        /// Appropriate error logs are generated for any inconsistencies detected in the file format or the checksum.
        /// </remarks>
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
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonString);
        }

        /// <summary>
        /// Asynchronously loads saveable objects from the provided data list, ordered by the 'Priority' key within each data dictionary.
        /// </summary>
        /// <param name="dataList">A dictionary mapping GUID strings to their associated data dictionary containing object field values and priorities.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// For each data dictionary in the provided list, this method:
        /// 1. Orders the dataList by the 'Priority' key.
        /// 2. Looks up the corresponding ISaveable object using the provided GUID.
        /// 3. Converts and sets the object's field values from the data dictionary.
        /// 4. Asynchronously loads the object with its updated data.
        /// Debug logs are generated during the process, and errors are logged if a given GUID does not correspond to a known ISaveable object.
        /// </remarks>
        private async Task LoadSaveableObjectsFromDataList(Dictionary<string, Dictionary<string, object>> dataList)
        {
            // Ordering dataList by Priority before loading each object
            var orderedDataList = dataList
                                  .OrderBy(dataDict => Convert.ToInt32(dataDict.Value["Priority"]))
                                  .ToList();

            foreach (var kvp in orderedDataList)
            {
                var guid = kvp.Key;
                var dataDict = kvp.Value;

                Debug.Log(string.Format("Executing load for guid: {0}", guid));

                if (saveableObjects.TryGetValue(guid, out ISaveable saveableObject))
                {
                    // Create an instance of the appropriate SaveableData type.
                    var data = saveableObject.Save();
                    ConvertAndSetFieldValues(data, dataDict);

                    await saveableObject.LoadAsync(data);
                    saveableObject.Load(data);
                }
                else
                {
                    Debug.LogError($"Invalid GUID: {guid} or no ISaveable object associated with it.");
                }
            }
        }

        /// <summary>
        /// Converts the values from the provided dictionary and sets them to the corresponding fields of the specified data object.
        /// </summary>
        /// <param name="data">The object whose fields are to be set.</param>
        /// <param name="dataDict">A dictionary containing field names as keys and their respective values to be set.</param>
        /// <remarks>
        /// The method attempts to deserialize the values from the dictionary using JSON serialization before setting them to the fields of the 'data' object.
        /// If deserialization fails, the original value from the dictionary is used.
        /// </remarks>
        private static void ConvertAndSetFieldValues(object data, IReadOnlyDictionary<string, object> dataDict)
        {
            foreach (var field in data.GetType().GetFields())
            {
                if (dataDict.TryGetValue(field.Name, out object fieldValue))
                {
                    object convertedValue;
                    try
                    {
                        convertedValue = JsonConvert.DeserializeObject(fieldValue.ToString(), field.FieldType);
                    }
                    catch
                    {
                        convertedValue = fieldValue;
                    }

                    object convertedValueForField = Convert.ChangeType(convertedValue, field.FieldType);
                    field.SetValue(data, convertedValueForField);
                }
            }
        }

        #endregion Private Methods
    }
}