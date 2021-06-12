using System.Xml;
using UnityEngine;

namespace Localization
{
    public class Localization : MonoBehaviour
    {
        #region Singleton

        public static Localization Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                Initialize();
            }
        }

        #endregion
        
        public Languages localization;

        private TextAsset _messageDefaultResource;
        private XmlDocument _messageDefaultResourceDoc;

        private TextAsset _languagesResource;
        private XmlDocument _languagesResourceDoc;
    
        private TextAsset _messageLocalizedResource;
        private XmlDocument _messageLocalizedResourceDoc;
        
        private void Initialize()
        {
            //Load default language into memory
            _messageDefaultResource = UnityEngine.Resources.Load<TextAsset>("Localization/Messages/Messages");
            _messageDefaultResourceDoc = new XmlDocument();
            _messageDefaultResourceDoc.LoadXml(_messageDefaultResource.text);
            _messageLocalizedResourceDoc = _messageDefaultResourceDoc;
        
            //Load language options
            _languagesResource = UnityEngine.Resources.Load<TextAsset>("Localization/Languages");
            _languagesResourceDoc = new XmlDocument();
            _languagesResourceDoc.LoadXml(_languagesResource.text);
            //TODO: Could probably create a dictionary of these values to be selected in game to replace the enum.
        
            if (localization == Languages.En) 
                return;
        
            //Load selected language into memory
            var languageTag = GetLanguageTag(localization);
            _messageLocalizedResource = UnityEngine.Resources.Load<TextAsset>($"Localization/Messages/Messages.{languageTag}");
            if (_messageLocalizedResource == null)
            {
                Debug.LogError($"{nameof(Localization)}: Language file not found for {localization.ToString()} tag: {languageTag}");
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
        
        private string GetLanguageTag(Languages language)
        {
            var valueTag = _languagesResourceDoc.SelectSingleNode($"//data[@name='{language.ToString()}']/value");
            return valueTag?.InnerText;
        }
        
        private void TextNotFoundError(string dataName)
        {
            Debug.LogError($"{nameof(Localization)}: Translation not found for {dataName} in {localization.ToString()}");
        }
        
        private void DefaultTextNotFoundError(string dataName)
        {
            Debug.LogError($"{nameof(Localization)}: Default text not found for {dataName} in En");
        }
    }
}