using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Localization.Models;
using UnityEngine;

namespace Localization
{
    public static class LocalizationManager
    {
        private static readonly Languages Localization; //currently set language

        private static TextAsset _languagesResource; //Languages.xml text to be parsed
        private static XmlDocument _languagesResourceDoc; //parsed Languages.xml

        private static TextAsset _messageDefaultResource; //messages.xml text to be parsed
        private static XmlDocument _messageDefaultResourceDoc; //parsed messages.xml

        private static TextAsset _messageLocalizedResource; //selected language .xml text to be parsed
        private static XmlDocument _messageLocalizedResourceDoc; //parsed selected language .xml


        private const string LanguagePrefKey = "language"; //PlayerPrefs key 

        static LocalizationManager()
        {
            var languagePref = PlayerPrefs.GetString(LanguagePrefKey, "En");
            Localization = (Languages) Enum.Parse(typeof(Languages), languagePref);
            Initialize();
        }

        /// <summary>
        /// Used to load a newly selected language into memory.
        /// This should typically only be called by the GameManager as this only reloads data in memory,
        /// UI refreshes are still needed, which is handled in the GameManager
        /// </summary>
        /// <param name="language"></param>
        public static void ChangeLanguage(Languages language)
        {
            PlayerPrefs.SetString(LanguagePrefKey, language.ToString());
            Initialize();
        }

        /// <summary>
        /// Gets the translated text in the set language based on the provided key parameter.
        /// </summary>
        /// <param name="key">Key of text</param>
        /// <returns>String of the localized text</returns>
        public static string GetTranslatedString(string key)
        {
#if (UNITY_EDITOR)
            var regexItem = new Regex("^[a-zA-Z0-9_-]*$");
            if (!regexItem.IsMatch(key))
            {
                Debug.LogError($"{nameof(Localization)}: Localization Keys can only contain a-z, 0-9, '_', and '-' \n  {key}");
                return "LOCALIZATION_KEY_ERROR";
            }
#endif

            //Localized
            var localizedValueTag = _messageLocalizedResourceDoc.SelectSingleNode($"//data[@name='{key}']/value");
            if (!string.IsNullOrEmpty(localizedValueTag?.InnerText)) return localizedValueTag.InnerText;

            //Default to english
            TextNotFoundError(key);
            localizedValueTag = _messageDefaultResourceDoc.SelectSingleNode($"//data[@name='{key}']/value");
            if (!string.IsNullOrEmpty(localizedValueTag?.InnerText)) return localizedValueTag?.InnerText;

            //Default text not found
            DefaultTextNotFoundError(key);
            return "TEXT_MISSING";
        }

        /// <summary>
        /// This method is not used in game, unity editor use only.
        /// Creates a dictionary of all the keys and text in the (default) Messages.xml file.
        /// </summary>
        /// <returns></returns>
        public static List<LocalizedText> LoadLocalizedDictionary()
        {
            //Load file
            var doc = new XmlDocument();
            doc.Load($"{Application.dataPath}/Resources/Localization/Messages/Messages.xml");

            //Load data
            var localizationDictionary = new List<LocalizedText>();
            var localizedValueTags = doc.SelectNodes($"//data");
            if (localizedValueTags == null || localizedValueTags.Count <= 0) return localizationDictionary;
            for (var i = 0; i < localizedValueTags.Count; i++)
            {
                var tag = localizedValueTags[i];
                var value = tag.SelectSingleNode("value");
                if (tag.Attributes != null && value != null)
                {
                    localizationDictionary.Add(new LocalizedText
                    {
                        key = tag.Attributes["name"].InnerText,
                        text = value.InnerText
                    });
                }
            }

            return localizationDictionary;
        }

        /// <summary>
        /// Loads both the default language and set language into memory from the localization files in Resources
        /// </summary>
        private static void Initialize()
        {
            //Load default language into memory
            _messageDefaultResource = Resources.Load<TextAsset>("Localization/Messages/Messages");
            _messageDefaultResourceDoc = new XmlDocument();
            _messageDefaultResourceDoc.LoadXml(_messageDefaultResource.text);
            _messageLocalizedResourceDoc = _messageDefaultResourceDoc;

            //Load language options
            _languagesResource = Resources.Load<TextAsset>("Localization/Languages");
            _languagesResourceDoc = new XmlDocument();
            _languagesResourceDoc.LoadXml(_languagesResource.text);
            //TODO: Could probably create a dictionary of these values to be selected in game to replace the enum.

            if (Localization == Languages.En)
                return;

            //Load selected language into memory
            var languageTag = GetLanguageTag(Localization);
            _messageLocalizedResource = Resources.Load<TextAsset>($"Localization/Messages/Messages.{languageTag}");
            if (_messageLocalizedResource == null)
            {
                Debug.LogError(
                    $"{nameof(Localization)}: Language file not found for {Localization.ToString()} tag: {languageTag}");
                _messageLocalizedResourceDoc = _messageDefaultResourceDoc; //Default language to english
            }
            else
            {
                _messageLocalizedResourceDoc = new XmlDocument();
                _messageLocalizedResourceDoc.LoadXml(_messageLocalizedResource.text);
            }
        }

        /// <summary>
        /// Load the language details based on the parameter from the Languages.xml file in Resources
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        private static string GetLanguageTag(Languages language)
        {
            var valueTag = _languagesResourceDoc.SelectSingleNode($"//data[@name='{language.ToString()}']/value");
            return valueTag?.InnerText;
        }

        /// <summary>
        /// Error when translation is missing from localized file
        /// </summary>
        /// <param name="dataName"></param>
        private static void TextNotFoundError(string dataName)
        {
            Debug.LogError($"{nameof(Localization)}: Translation not found for {dataName} in {Localization.ToString()}");
        }

        /// <summary>
        /// Error when default text is missing from the default localization file
        /// </summary>
        /// <param name="dataName"></param>
        private static void DefaultTextNotFoundError(string dataName)
        {
            Debug.LogError($"{nameof(Localization)}: Default text not found for {dataName} in En");
        }
    }
}