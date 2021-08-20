using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Saving.Models;
using UnityEngine;

namespace Saving
{
    public class SaveManager : MonoBehaviour
    {
        public GameState gameState;
        private string _savePath;

        private void Start()
        {
            _savePath = $"{Application.persistentDataPath}/saves";
        }

        /// <summary>
        /// autoSave and fileName are both optional parameters.
        /// autoSave will always overwrite the default save file auto.el
        /// fileName allows the user to overwrite an existing save
        /// If neither parameters are provided a new save file will be created
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="autoSave"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool SaveGame(GameState gameState, bool autoSave = false, string fileName = null)
        {
            try
            {
                if (!Directory.Exists(_savePath))
                {
                    Directory.CreateDirectory(_savePath);
                }

                if (autoSave)
                {
                    fileName = "auto";
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    //Not throwing an overwrite warning here, needs to be handled frontend
                    return CreateSaveFile(gameState, fileName);
                }

                var saveCount = Directory.GetFiles(_savePath, "*", SearchOption.TopDirectoryOnly).Length;
                return CreateSaveFile(gameState, $"sav_{saveCount}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save game: '{exception.Message}'");
                return false;
            }
        }

        /// <summary>
        /// Based on the provided file path, loads the save and deserializes into the gameState object
        /// that is returned.
        /// </summary>
        /// <param name="path">File to open</param>
        /// <returns></returns>
        public GameState LoadGame(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
                serializer.UnknownNode += SerializerUnknownNode;
                serializer.UnknownAttribute += SerializerUnknownAttribute;

                var fileStream = new FileStream(path, FileMode.Open);
                var gameStateEl = (GameState)serializer.Deserialize(fileStream);
                fileStream.Close();
                return gameStateEl;
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save game: '{exception.Message}'");
                return null;
            }
        }

        /// <summary>
        /// Gets a list of all the saves and returns the name, path and date of creation
        /// </summary>
        /// <returns></returns>
        public List<SaveFile> GetSaveFilesDetails()
        {
            var saveFiles = new List<SaveFile>();
            if (!Directory.Exists(_savePath)) return saveFiles;
            var directoryInfo = new DirectoryInfo(_savePath);
            var files = directoryInfo.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            saveFiles.AddRange(files.Select(file => new SaveFile
                { fileName = file.Name, filePath = file.FullName, saveDate = file.CreationTime }));
            return saveFiles;
        }

        /// <summary>
        /// Serializes the GameState gameobject and creates a save file based on the provided file name
        /// All files are created in the persistent data path
        /// </summary>
        /// <param name="gameState">Object to be serialized</param>
        /// <param name="fileName">Name of save</param>
        /// <returns></returns>
        private bool CreateSaveFile(GameState gameState, string fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
                var path = $"{Application.persistentDataPath}/saves/{fileName}.el";
                var streamWriter = new StreamWriter(path);
                serializer.Serialize(streamWriter, gameState);
                streamWriter.Close();
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save game: '{exception.Message}'");
                return false;
            }
        }

        /// <summary>
        /// Catches exceptions for unknown nodes in save files.
        /// Possible causes is save file was modified outside of game or the GameState model was updated
        /// and the save is outdated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
        {
            Debug.LogError($"{nameof(SaveManager)}: Unknown Node: {e.Name} \t {e.Text}");
        }

        /// <summary>
        /// Catches exceptions for unknown attributes in save files.
        /// Possible causes is save file was modified outside of game or the GameState model was updated
        /// and the save is outdated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Debug.LogError($"{nameof(SaveManager)}: Unknown attribute {e.Attr.Name} ='{e.Attr.Value}'");
        }
    }
}