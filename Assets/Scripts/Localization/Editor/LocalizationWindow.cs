using System;
using System.Collections.Generic;
using System.Linq;
using Localization.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Tools = EditorTools.Tools;

namespace Localization.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        private LocalizationManager _localizationManager;

        private List<LocalizedText> _localizedTexts = new List<LocalizedText>();
        public string filterValue;
        public Vector2 scroll;

        [MenuItem("Tools/CheddyShakes/Localization")]
        static void Init()
        {
            var window = (LocalizationWindow) EditorWindow.GetWindow(typeof(LocalizationWindow));
            window.titleContent = new GUIContent("Localization");
            window.Show();
        }


        private void OnEnable()
        {
            _localizationManager = new LocalizationManager(Languages.En);
            _localizedTexts = _localizationManager.LoadAll();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("Search: ", EditorStyles.boldLabel);
            filterValue = EditorGUILayout.TextField(filterValue);

            if (GUILayout.Button("Refresh"))
            {
                _localizedTexts.Clear();
                _localizedTexts = _localizationManager.LoadAll();
            }

            EditorGUILayout.EndHorizontal();
            GetSearchResults();
        }

        private void GetSearchResults()
        {
            EditorGUILayout.BeginVertical();
            scroll = EditorGUILayout.BeginScrollView(scroll);

            if (_localizedTexts != null && _localizedTexts.Count > 0)
            {
                var filteredDictionary = _localizedTexts;
                if (!string.IsNullOrEmpty(filterValue))
                {
                    filteredDictionary = new List<LocalizedText>();
                    foreach (var localizedText in _localizedTexts.Where(localizedText => localizedText != null))
                    {
                        if (!string.IsNullOrEmpty(localizedText.key) &&
                            localizedText.key.ToUpper().Contains(filterValue.ToUpper()))
                        {
                            filteredDictionary.Add(localizedText);
                        }
                        else if (!string.IsNullOrEmpty(localizedText.text) &&
                                 localizedText.text.ToUpper().Contains(filterValue.ToUpper()))
                        {
                            filteredDictionary.Add(localizedText);
                        }
                    }
                }
 

                foreach (var element in filteredDictionary)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                    {
                        if (EditorUtility.DisplayDialog(
                            $"Remove key {element.key}?", $"This will remove the {element.key} from localization." +
                                                          $"Please ensure that all references to this key have been removed before deleting",
                            "Ok",
                            "Cancel"))
                        {
                            Tools.DeleteMessage(element.key);
                            _localizedTexts = _localizationManager.LoadAll();
                        }
                    }

                    EditorGUILayout.LabelField(element.key, GUILayout.MaxWidth(150));
                    EditorGUILayout.TextField(element.text);
                    EditorGUILayout.EndHorizontal();
                }
            }


            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}