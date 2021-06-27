using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Models.Saving;
using UnityEngine;

namespace Managers
{
    public static class SaveManager
    {
        private static readonly string SavePath;

        static SaveManager()
        {
            SavePath = $"{Application.persistentDataPath}/saves";
        }
        
        public static bool SaveGame(GameState gameState, bool autoSave = false)
        {
            try
            {
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
                }

                if (autoSave)
                {
                    return CreateSaveFile(gameState, "auto");
                }

                var saveCount = Directory.GetFiles(SavePath, "*", SearchOption.TopDirectoryOnly).Length;
                return CreateSaveFile(gameState, $"sav_{saveCount}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save game: '{exception.Message}'");
                return false;
            }
        }
        
        public static GameState LoadGame(string path)
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
                var gameState = (GameState) serializer.Deserialize(fileStream);
                fileStream.Close();
                return gameState;
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save game: '{exception.Message}'");
                return null;
            }
        }

        public static List<SaveFile> GetSaveFileNames()
        {
            var saveFiles = new List<SaveFile>();
            if (!Directory.Exists(SavePath)) return saveFiles;
            var directoryInfo = new DirectoryInfo(SavePath);
            var files = directoryInfo.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            saveFiles.AddRange(files.Select(file => new SaveFile {fileName = file.Name, filePath = file.FullName, saveDate = file.CreationTime}));
            return saveFiles;
        }
        
        private static bool CreateSaveFile(GameState gameState, string fileName)
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

        private static void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
        {
            Debug.LogError($"{nameof(SaveManager)}: Unknown Node: {e.Name} \t {e.Text}");
        }

        private static void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Debug.LogError($"{nameof(SaveManager)}: Unknown attribute {e.Attr.Name} ='{e.Attr.Value}'");
        }
    }
}