using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Saving.Models;
using UnityEditor;
using UnityEngine;

namespace Saving.Editor
{
    public class SaveWindow : EditorWindow
    {
        public string newState;
        public Vector2 scroll;

        [MenuItem("Tools/CheddyShakes/Save Editor")]
        private static void Init()
        {
            var window = (SaveWindow)EditorWindow.GetWindow(typeof(SaveWindow));
            window.titleContent = new GUIContent("Save Editor");
            window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("New state: ", EditorStyles.boldLabel);
            newState = EditorGUILayout.TextField(newState);

            if (GUILayout.Button("Add"))
            {
                CreateNewEventState(newState);
            }

            EditorGUILayout.EndHorizontal();
            GetEventState();
        }

        private void GetEventState()
        {
            EditorGUILayout.BeginVertical();
            scroll = EditorGUILayout.BeginScrollView(scroll);

            // var eventStates = Enum.GetNames(typeof(EventStates));
            // if (eventStates.Length > 0)
            // {
            //     foreach (var eventState in eventStates)
            //     {
            //         EditorGUILayout.BeginHorizontal("box");
            //         EditorGUILayout.LabelField(eventState);
            //         EditorGUILayout.EndHorizontal();
            //     }
            // }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private static void CreateNewEventState(string eventStateName)
        {
            //Validate
            if (string.IsNullOrEmpty(eventStateName))
                return;

            eventStateName = eventStateName.Trim();
            if (Regex.IsMatch(eventStateName, "[^a-zA-Z\\d]"))
            {
                Debug.LogError(
                    $"{nameof(SaveWindow)}: EventState name cannot contain characters. AlphaNumeric only.");
                return;
            }
            
            //Const
            const string endTag = "//[END]";
            var fileName = $"{Application.dataPath}\\Scripts\\Saving\\Models\\EventStates.cs";
            
            //Parse
            var eventStateEnumLines = File.ReadAllLines(fileName).ToList();
            
            //End tag
            var endTagIndex = eventStateEnumLines.FindIndex(x => x.Contains(endTag));
            if (endTagIndex == 0)
            {
                Debug.LogError($"{nameof(SaveWindow)}: Could not find '//[END]' Please add it back after the last enum value.");
                return;
            }
            
            //Existing check
            var te = eventStateEnumLines.FirstOrDefault(x => x.Contains(eventStateName));
            if (!string.IsNullOrEmpty(te))
            {
                Debug.LogError($"{nameof(SaveWindow)}: Value already exists, duplicates cannot be created.");
                return;
            }
            
            //Add
            eventStateEnumLines.Insert(endTagIndex, $"{eventStateName},");
            File.WriteAllLines(fileName, eventStateEnumLines);
            
            //Unity doesn't detect this change unless we leave the window and come back.
            //Manually trigger build
            AssetDatabase.Refresh();
        }
    }
}