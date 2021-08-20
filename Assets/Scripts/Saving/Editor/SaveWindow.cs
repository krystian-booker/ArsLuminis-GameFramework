using System;
using System.IO;
using System.Xml.Serialization;
using Saving.Models;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Saving.Editor
{
    public class SaveWindow : EditorWindow
    {
        private static SaveWindow _saveWindow;
        private static SaveManager _saveManager;
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
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_saveManager);
                EditorSceneManager.MarkSceneDirty(_saveManager.gameObject.scene);
                Repaint();
            }
            
            if (_serializedObject == null)
            {
                if (GUILayout.Button("Reload scene & Refresh Window"))
                {
                    LoadAppScene();
                }
                return;
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
            _saveManager = app.GetComponent<SaveManager>();

            //Will be null after builds
            if (_saveWindow == null)
            {
                _saveWindow = (SaveWindow)GetWindow(typeof(SaveWindow));
            }

            _saveWindow._serializedObject = new SerializedObject(_saveManager);
        }

        private static void LoadSaveTemplate()
        {
            var path = EditorUtility.OpenFilePanel("Open save template", $"{Application.dataPath}/", "template");
            _saveManager.gameState = LoadTemplate(path);
            EditorUtility.SetDirty(_saveManager);
            EditorSceneManager.MarkSceneDirty(_saveManager.gameObject.scene);
        }
        
        private static void CreateSaveTemplate()
        {
            var path = EditorUtility.SaveFilePanel("Create template", $"{Application.dataPath}/", "saveFileFormat",
                "template");
            _saveManager.gameState = SanitizeSaveTemplate(_saveManager.gameState);
            CreateSaveTemplateFile(_saveManager.gameState, path);
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

        private static void CreateSaveTemplateFile(GameState gameState, string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GameState));
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