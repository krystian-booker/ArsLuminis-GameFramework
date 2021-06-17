using System;
using System.Collections.Generic;
using System.Xml;
using Localization.Models;
using UnityEngine;

namespace Localization
{
    public class LocalizationManager
    {
        private Languages _localization;

        private TextAsset _messageDefaultResource;
        private XmlDocument _messageDefaultResourceDoc;

        private TextAsset _languagesResource;
        private XmlDocument _languagesResourceDoc;

        private TextAsset _messageLocalizedResource;
        private XmlDocument _messageLocalizedResourceDoc;

        public LocalizationManager(Languages initialLanguage)
        {
            _localization = initialLanguage;
        }

        public void Initialize()
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

            if (_localization == Languages.En)
                return;

            //Load selected language into memory
            var languageTag = GetLanguageTag(_localization);
            _messageLocalizedResource = Resources.Load<TextAsset>($"Localization/Messages/Messages.{languageTag}");
            if (_messageLocalizedResource == null)
            {
                Debug.LogError(
                    $"{nameof(Localization)}: Language file not found for {_localization.ToString()} tag: {languageTag}");
                _messageLocalizedResourceDoc = _messageDefaultResourceDoc; //Default language to english
            }
            else
            {
                _messageLocalizedResourceDoc = new XmlDocument();
                _messageLocalizedResourceDoc.LoadXml(_messageLocalizedResource.text);
            }
        }

        public string GetTranslatedString(string dataName)
        {
            //Localized
            var localizedValueTag = _messageLocalizedResourceDoc.SelectSingleNode($"//data[@name='{dataName}']/value");
            if (!string.IsNullOrEmpty(localizedValueTag?.InnerText)) return localizedValueTag.InnerText;

            //Default to english
            TextNotFoundError(dataName);
            localizedValueTag = _messageDefaultResourceDoc.SelectSingleNode($"//data[@name='{dataName}']/value");
            if (!string.IsNullOrEmpty(localizedValueTag?.InnerText)) return localizedValueTag?.InnerText;

            //Default text not found
            DefaultTextNotFoundError(dataName);
            return "TEXT_MISSING";
        }

        public List<LocalizedText> LoadAll()
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

        private string GetLanguageTag(Languages language)
        {
            var valueTag = _languagesResourceDoc.SelectSingleNode($"//data[@name='{language.ToString()}']/value");
            return valueTag?.InnerText;
        }

        private void TextNotFoundError(string dataName)
        {
            Debug.LogError(
                $"{nameof(Localization)}: Translation not found for {dataName} in {_localization.ToString()}");
        }

        private void DefaultTextNotFoundError(string dataName)
        {
            Debug.LogError($"{nameof(Localization)}: Default text not found for {dataName} in En");
        }
    }
}