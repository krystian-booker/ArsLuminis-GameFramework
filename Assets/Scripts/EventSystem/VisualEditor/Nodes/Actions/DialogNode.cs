using System.Collections.Generic;
using EditorTools;
using EventSystem.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#277DA1")]
    public class DialogNode : BaseNode
    {
        #region XNode

        [Input] public NodeLink entry;

        [Output, HideIf("@this.options.Count > 0")]
        public NodeLink exit;

        /// <summary>
        /// Unused, required by xNode
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }

        #endregion

        #region Speaker

        [Header("Speaker properties")]
        
        [LabelWidth(100), Tooltip("Used for character name, arrow position")]
        public GameObject character;

        //TODO: Remove once character work is complete
        [LabelWidth(200), Tooltip("Temporary, will be removed once character work is complete")]
        public string characterName;
        
        [LabelWidth(200), Tooltip("When enabled the dialog box will follow the speaker")]
        public bool followCharacter;
        
        [LabelWidth(200), Tooltip("Enable for timePerCharacter to be used")]
        public bool customTimePerCharacter;

        [LabelWidth(200), Tooltip("Used to set a custom time per character, if not set default will be used")]
        public int timePerCharacter;

        #endregion

        #region DialogLayout

        [Header("Dialog box properties")]
        
        [LabelWidth(200), Tooltip("Enable for dialogPosition to be used")]
        public bool customDialogPosition;

        [ShowInInspector, LabelWidth(200), Tooltip("Used to set the X position in the canvas of the dialog instance")]
        public int dialogPositionX = -1;

        [ShowInInspector, LabelWidth(200), Tooltip("Used to set the Y position in the canvas of the dialog instance")]
        public int dialogPositionY = -1;

        [ShowInInspector, LabelWidth(200), Tooltip("Sets the width of the dialog window, when 0 default will be used")]
        public int dialogWidth;

        [ShowInInspector, LabelWidth(200), Tooltip("Sets the height of the dialog window, when 0 default will be used")]
        public int dialogHeight;

        #endregion

        #region DialogTextLocalization

        [LabelWidth(100)] [Tooltip("User for localization")]
        public string key;

        private string _lastKey; //Used to store the last state of the key

        [TextArea] public string text;

        //The maximum ports is a soft limit. This is defined by the amount of options setup in the dialog manager
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
            if (string.IsNullOrEmpty(key) || !string.IsNullOrEmpty(_lastKey) && !_lastKey.Equals(key))
            {
                text = string.Empty;
            }

            if (!string.IsNullOrEmpty(key))
            {
                var message = Tools.GetMessage(key);
                if (string.IsNullOrEmpty(message))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        Tools.UpdateMessage(key, text);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(text) && !text.Equals(message))
                    {
                        Tools.UpdateMessage(key, text);
                    }
                    else
                    {
                        text = message;
                    }
                }
            }

            _lastKey = key;
        }
#endif

        #endregion
    }
}