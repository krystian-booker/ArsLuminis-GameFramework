using System;
using System.IO;
using System.Xml.Serialization;
using Saving.Models;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Saving.Editor
{
    public class SaveWindow : EditorWindow
    {
        private static GameState _gameState;
        private static SaveWindow _saveWindow;
        private SerializedObject _serializedObject;

        public string newState;
        public Vector2 scroll;

        [MenuItem("Tools/CheddyShakes/Save Editor")]
        private static void Init()
        {
            _saveWindow = (SaveWindow)GetWindow(typeof(SaveWindow));
            _saveWindow.titleContent = new GUIContent("Save Editor");
            _saveWindow.Show();
            LoadAppScene();
        }

        public void OnGUI()
        {
            if (_serializedObject == null)
            {
                LoadAppScene();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Template"))
            {
                LoadSaveTemplate();
            }

            if (GUILayout.Button("Save Template"))
            {
                CreateSaveTemplate();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            var enterChildren = true;
            var iterator = _serializedObject.GetIterator();
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "m_Script") continue;
                switch (iterator.name)
                {
                    case "gameState":
                        if (Event.current.type == EventType.Repaint)
                        {
                            if (iterator.serializedObject.targetObject == null)
                            {
                                if (GUILayout.Button("Reload scene & Refresh Window"))
                                {
                                    LoadAppScene();
                                }

                                return;
                            }
                        }

                        EditorGUILayout.PropertyField(iterator);
                        break;
                    default:
                        EditorGUILayout.PropertyField(iterator);
                        break;
                }
            }
        }

        private static void LoadAppScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/_preload.unity");
            var app = GameObject.Find("__app");
            var saveManager = app.GetComponent<SaveManager>();

            //Will be null after builds
            if (_saveWindow == null)
            {
                _saveWindow = (SaveWindow)GetWindow(typeof(SaveWindow));
            }

            _saveWindow._serializedObject = new SerializedObject(saveManager);
        }

        private static void LoadSaveTemplate()
        {
            var path = EditorUtility.OpenFilePanel("Open save template", $"{Application.dataPath}/", "template");
            _gameState = LoadTemplate(path);
        }
        
        private static void CreateSaveTemplate()
        {
            var path = EditorUtility.SaveFilePanel("Create template", $"{Application.dataPath}/", "saveFileFormat",
                "template");
            _gameState = SanitizeSaveTemplate(_gameState);
            CreateSaveTemplateFile(_gameState, path);
        }
        
        private static GameState SanitizeSaveTemplate(GameState gameState)
        {
            //Sanitize the values as templates should be be storing save data
            foreach (var state in gameState.states)
            {
                state.stringValue = string.Empty;
                state.intValue = 0;
                state.floatValue = 0f;
                state.booleanValue = false;
                state.vector3Value = Vector3.zero;
            }

            return gameState;
        }

        private static void CreateSaveTemplateFile(GameState gameState, string fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
                var path = $"{Application.persistentDataPath}/saves/{fileName}.el";
                var streamWriter = new StreamWriter(path);
                serializer.Serialize(streamWriter, gameState);
                streamWriter.Close();
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable save template: '{exception.Message}'");
            }
        }
        
        private static GameState LoadTemplate(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
                var fileStream = new FileStream(path, FileMode.Open);
                var gameState = (GameState)serializer.Deserialize(fileStream);
                fileStream.Close();
                return gameState;
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(SaveManager)}: Unable to load template: '{exception.Message}'");
                return null;
            }
        }
    }
}