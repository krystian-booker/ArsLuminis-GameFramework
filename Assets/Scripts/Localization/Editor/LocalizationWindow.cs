using System.Collections.Generic;
using System.Linq;
using Localization.Models;
using Tools;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    public class LocalizationWindow : EditorWindow
    {
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

        /// <summary>
        /// Loads localized dictionary from the (default)Messages.xml 
        /// </summary>
        private void OnEnable()
        {
            _localizedTexts = LocalizationManager.LoadLocalizedDictionary();
        }

        /// <summary>
        /// Generates Unity UI from "Localization" window
        /// Using data dictionary from the localizationManager
        /// </summary>
        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("Search: ", EditorStyles.boldLabel);
            filterValue = EditorGUILayout.TextField(filterValue);

            if (GUILayout.Button("Refresh"))
            {
                _localizedTexts.Clear();
                _localizedTexts = LocalizationManager.LoadLocalizedDictionary();
            }

            EditorGUILayout.EndHorizontal();
            GetSearchResults();
        }

        /// <summary>
        /// Generates Unity UI from "Localization" window, specifically the scroll section
        /// Using data dictionary from the localizationManager
        /// </summary>
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
                            Utilities.DeleteMessage(element.key);
                            _localizedTexts = LocalizationManager.LoadLocalizedDictionary();
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