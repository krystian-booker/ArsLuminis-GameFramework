using System.Collections.Generic;
using EventSystem.Models;
using EventSystem.VisualEditor.Nodes.Dialog.Models;
// using Sirenix.OdinInspector;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace EventSystem.VisualEditor.Nodes.Dialog
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#277DA1")]
    public class DialogNode : BaseNode
    {
        #region XNode

        [Input] public NodeLink entry;

        [Output] public NodeLink exit;

        #endregion

        #region Speaker

        [Header("Speaker properties")]
        [Tooltip("Required for dialog, used for tracking, names, sizing, location, etc.")]
        public GameObject character;

        #endregion

        #region Dialog Configuration

        [Header("Dialog Configuration")] [Tooltip("When enabled the dialog box will follow the speaker")]
        public bool followCharacter;

        [Tooltip(
            "When enabled the dialog box will not require the user to click confirm, the dialog will display for 'n' amount of time.")]
        public bool displayForNTime;

        [Tooltip("Time that the dialog box will display in SECONDS, timer starts AFTER the text is fully displayed")]
        public int displayTime;

        [Tooltip("Enable for timePerCharacter to be used")]
        public bool customTimePerCharacter;

        [Tooltip("Used to set a custom time per character, if not set default will be used")]
        public int timePerCharacter;

        #endregion

        #region Dialog Layout

        [Header("Dialog properties")] [Tooltip("Enable for dialogPosition to be used")]
        public bool customDialogPosition;

        [Tooltip("Used to set the X position in the canvas of the dialog instance")]
        public int dialogPositionX = -1;

        [Tooltip("Used to set the Y position in the canvas of the dialog instance")]
        public int dialogPositionY = -1;

        [Tooltip("Sets the width of the dialog window, when 0 default will be used")]
        public int dialogWidth = 1000;

        [Tooltip("Sets the height of the dialog window, when 0 default will be used")]
        public int dialogHeight = 300;

        #endregion

        #region DialogTextLocalization

        [Tooltip("Localization key")] public string localizationKey;
        private string _lastLocalizationKey; //Used to store the last state of the key

        [TextArea] public string text;

        [SerializeField, Output(dynamicPortList = true),
         Tooltip("Options user has to select, maximum ports depends on amount defined on UI")]
        public List<DialogOption> options = new List<DialogOption>();

        //These aren't ideal but are required as a result of the xNode cloning element issue
        [HideInInspector] public int nCount;
        [HideInInspector] public List<string> optionsKeyTracker = new List<string>();

#if UNITY_EDITOR
        /// <summary>
        /// Once a key has been entered, check if it exists in the (default)messages.xml
        /// If it does, load the text.
        /// If not create the entry
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(localizationKey) ||
                !string.IsNullOrEmpty(_lastLocalizationKey) && !_lastLocalizationKey.Equals(localizationKey))
            {
                text = string.Empty;
            }

            if (!string.IsNullOrEmpty(localizationKey))
            {
                var message = Utilities.GetMessage(localizationKey);
                if (string.IsNullOrEmpty(message))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        Utilities.UpdateMessage(localizationKey, text);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(text) && !text.Equals(message))
                    {
                        Utilities.UpdateMessage(localizationKey, text);
                    }
                    else
                    {
                        text = message;
                    }
                }
            }

            _lastLocalizationKey = localizationKey;
        }
#endif

        #endregion
    }
}