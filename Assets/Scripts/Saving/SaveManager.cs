using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using EventSystem;
using EventSystem.VisualEditor.Nodes.State;
using Saving.Models;
using Tools;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace Saving
{
    public class SaveManager : MonoBehaviour
    {
        public GameState saveTemplate;
        private GameState _gameState;

        private string _savePath;

        private void Start()
        {
            _savePath = $"{Application.persistentDataPath}/saves";
            _gameState = saveTemplate;
        }

        /// <summary>
        /// Update the selected state by id to the newly set value
        /// </summary>
        /// <param name="updateStateNode"></param>
        public void UpdateState(UpdateStateNode updateStateNode)
        {
            var eventStateValue = _gameState.states.FirstOrDefault(x => x.id == updateStateNode.selectedStateId);
            Assert.IsNotNull(eventStateValue, $"{nameof(SaveManager)}: Unable to find the state '{updateStateNode.selectedStateId}'");
            switch (eventStateValue.dataType)
            {
                case DataType.String:
                    eventStateValue.stringValue = updateStateNode.stringValue;
                    break;
                case DataType.Integer:
                    eventStateValue.intValue = updateStateNode.intValue;
                    break;
                case DataType.Float:
                    eventStateValue.floatValue = updateStateNode.floatValue;
                    break;
                case DataType.Boolean:
                    eventStateValue.booleanValue = updateStateNode.booleanValue;
                    break;
                case DataType.Vector3:
                    eventStateValue.vector3Value = updateStateNode.vector3Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Execution of the state branch node, determines datatype of the selected eventState.
        /// Finds a matching value from the provided outputs in the state branch node.
        /// If no matching value is found the default port will be used.
        /// </summary>
        /// <param name="node">To be execute</param>
        /// <returns>List of ports to execute</returns>
        public static List<NodePort> ExecuteStateBranchNode(Node node)
        {
            var stateNode = node as StateBranchNode;
            Assert.IsNotNull(stateNode);

            var eventState = Systems.saveManager._gameState.states.FirstOrDefault(x => x.id == stateNode.selectedStateId);
            Assert.IsNotNull(eventState, $"{nameof(EventSequenceParser)}: Unable to find the state '{stateNode.selectedStateId}' in gameManager states");

            NodePort nodePort = null;
            var dynamicOutputs = node.DynamicOutputs;
            switch (eventState.dataType)
            {
                case DataType.String:
                    var stringStateIndex = stateNode.stringOptions.FindIndex(x => x == eventState.stringValue);
                    nodePort = dynamicOutputs.FirstOrDefault(x => x.fieldName == $"{nameof(StateBranchNode.stringOptions)} {stringStateIndex}");
                    break;
                case DataType.Integer:
                    var intStateIndex = stateNode.stringOptions.FindIndex(x => x == eventState.stringValue);
                    nodePort = dynamicOutputs.FirstOrDefault(x => x.fieldName == $"{nameof(StateBranchNode.integerOptions)} {intStateIndex}");
                    break;
                case DataType.Float:
                    var floatStateIndex = stateNode.stringOptions.FindIndex(x => x == eventState.stringValue);
                    nodePort = dynamicOutputs.FirstOrDefault(x => x.fieldName == $"{nameof(StateBranchNode.floatOptions)} {floatStateIndex}");
                    break;
                case DataType.Boolean:
                    var boolOutput = eventState.booleanValue ? nameof(StateBranchNode.valueTrue) : nameof(StateBranchNode.valueFalse);
                    nodePort = node.Ports.FirstOrDefault(portNode => portNode.fieldName == boolOutput);
                    break;
                case DataType.Vector3:
                    var vecStateIndex = stateNode.stringOptions.FindIndex(x => x == eventState.stringValue);
                    nodePort = dynamicOutputs.FirstOrDefault(x => x.fieldName == $"{nameof(StateBranchNode.vector3Options)} {vecStateIndex}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            nodePort ??= node.Ports.FirstOrDefault(x => x.fieldName == nameof(StateBranchNode.defaultOutput));
            Assert.IsNotNull(nodePort);
            return nodePort.GetConnections();
        }

        /// <summary>
        /// autoSave and fileName are both optional parameters.
        /// autoSave will always overwrite the default save file auto.el
        /// fileName allows the user to overwrite an existing save
        /// If neither parameters are provided a new save file will be created
        /// </summary>
        /// <param name="autoSave"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool SaveGame(bool autoSave = false, string fileName = null)
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
                    return CreateSaveFile(fileName);
                }

                var saveCount = Directory.GetFiles(_savePath, "*", SearchOption.TopDirectoryOnly).Length;
                return CreateSaveFile($"sav_{saveCount}");
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
        /// <param name="fileName">File to open</param>
        /// <returns></returns>
        public void LoadGame(string fileName)
        {
            var file = $"{_savePath}/{fileName}";
            if (!File.Exists(file))
                return;

            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
                serializer.UnknownNode += SerializerUnknownNode;
                serializer.UnknownAttribute += SerializerUnknownAttribute;

                var fileStream = new FileStream(file, FileMode.Open);
                _gameState = serializer.Deserialize(fileStream) as GameState;

                fileStream.Close();
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save game: '{exception.Message}'");
            }
        }

        /// <summary>
        /// Gets a list of all the saves and returns the name, path and date of creation
        /// </summary>
        /// <returns></returns>
        public List<SaveFile> GetSaveFilesDetails()
        {
            var saveFiles = new List<SaveFile>();
            if (!Directory.Exists(_savePath))
                return saveFiles;

            var directoryInfo = new DirectoryInfo(_savePath);
            var files = directoryInfo.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            saveFiles.AddRange(files.Select(file => new SaveFile
            {
                fileName = file.Name,
                filePath = file.FullName,
                saveDate = file.CreationTime
            }));

            return saveFiles;
        }

        /// <summary>
        /// Serializes the GameState gameobject and creates a save file based on the provided file name
        /// All files are created in the persistent data path
        /// </summary>
        /// <param name="fileName">Name of save</param>
        /// <returns></returns>
        private bool CreateSaveFile(string fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
                var path = $"{Application.persistentDataPath}/saves/{fileName}.el";
                var streamWriter = new StreamWriter(path);
                serializer.Serialize(streamWriter, _gameState);
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