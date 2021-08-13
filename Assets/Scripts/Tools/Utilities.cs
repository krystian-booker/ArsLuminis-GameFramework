using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using EventSystem.VisualEditor.Nodes.Dialog.Models;
using UnityEngine;

namespace Tools
{
    public class Utilities : MonoBehaviour
    {
        /// <summary>
        /// Used to instantiate game objects from in editor.
        /// Example: used to create Targets from prefabs
        /// </summary>
        /// <param name="gameObject">Object to be instantiated</param>
        /// <returns>Reference to the instantiated gameobject</returns>
        public static GameObject InstantiateObject(GameObject gameObject)
        {
            Debug.Log($"Object instantiated at 0,0,0");
            return Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Used from the removal of game objects from none MonoBehaviour classes
        /// </summary>
        /// <param name="component"></param>
        public static void DestroyComponent(Component component)
        {
            Destroy(component);
        }

        /// <summary>
        /// Finds enum from a string of the enums name.
        /// This should only be used for editor code and NEVER in game.
        /// </summary>
        /// <param name="enumName">Name of the enum</param>
        /// <returns>enum</returns>
        public static Type GetEnumType(string enumName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(enumName))
                .Where(type => type != null).FirstOrDefault(type => type.IsEnum);
        }

        /// <summary>
        /// Called from the unity editor to update the Messages.xml with new messages.
        /// </summary>
        /// <param name="key">Typically a guid unless coming from manually created UI elements</param>
        /// <param name="message">Text to be added/updated</param>
        public static void UpdateMessage(string key, string message)
        {
            //Prevents initial add from UI keys
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(message))
                return;

            //Load document
            var messageDefaultResource = Resources.Load<TextAsset>("Localization/Messages/Messages");
            var messageDefaultResourceDoc = new XmlDocument();
            messageDefaultResourceDoc.LoadXml(messageDefaultResource.text);

            //Check if key exists to edit if not add
            var localizedValueTag = messageDefaultResourceDoc.SelectSingleNode($"//data[@name='{key}']/value");
            if (localizedValueTag == null)
            {
                //Create data element, add resx required properties
                var data = messageDefaultResourceDoc.CreateElement("data");
                data.SetAttribute("name", key);
                data.SetAttribute("xml:space", "preserve");

                //Add value
                var value = messageDefaultResourceDoc.CreateElement("value");
                value.InnerText = message;

                //Add to original document
                data.AppendChild(value);
                messageDefaultResourceDoc.DocumentElement?.AppendChild(data);
            }
            //Only update text if text changed
            else if (!localizedValueTag.InnerText.Equals(message))
            {
                localizedValueTag.InnerText = message;
            }

            messageDefaultResourceDoc.Save("Assets/Resources/Localization/Messages/Messages.xml");
        }

        /// <summary>
        /// Called from the unity editor to update the Messages.xml with new messages from options
        /// Since we can't use the delayed attribute on options this prevents extreme rapid loading of the xml files
        /// Unfortunately there is still a lot of unnecessary loading that is happening here that I should probably fix
        /// </summary>
        /// <param name="options">List of options from dialog that will be updated</param>
        public static void BulkOptionUpdateMessages(List<DialogOption> options)
        {
            if (options == null || options.Count <= 0)
                return;

            //Load document
            var messageDefaultResource = Resources.Load<TextAsset>("Localization/Messages/Messages");
            var messageDefaultResourceDoc = new XmlDocument();
            messageDefaultResourceDoc.LoadXml(messageDefaultResource.text);

            foreach (var option in options)
            {
                if (string.IsNullOrEmpty(option.key))
                    continue;

                //Check if key exists to edit if not add
                var localizedValueTag =
                    messageDefaultResourceDoc.SelectSingleNode($"//data[@name='{option.key}']/value");
                if (localizedValueTag == null)
                {
                    //Skip if no message to add
                    if (string.IsNullOrEmpty(option.text))
                        continue;
                    
                    //Create data element, add resx required properties
                    var data = messageDefaultResourceDoc.CreateElement("data");
                    data.SetAttribute("name", option.key);
                    data.SetAttribute("xml:space", "preserve");

                    //Add value
                    var value = messageDefaultResourceDoc.CreateElement("value");
                    value.InnerText = option.text;

                    //Add to original document
                    data.AppendChild(value);
                    messageDefaultResourceDoc.DocumentElement?.AppendChild(data);
                }
                else if (string.IsNullOrEmpty(option.text))
                {
                    option.text = localizedValueTag.InnerText;
                }
                //Update resx if text in option is different and not empty
                else if (localizedValueTag.InnerText != option.text)
                {
                    localizedValueTag.InnerText = option.text;
                }

                messageDefaultResourceDoc.Save("Assets/Resources/Localization/Messages/Messages.xml");
            }
        }

        /// <summary>
        /// Used to get the text for a key from the (default)Messages.xml
        /// in the Unity editor
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMessage(string key)
        {
            //Load document
            var messageDefaultResource = Resources.Load<TextAsset>("Localization/Messages/Messages");
            var messageDefaultResourceDoc = new XmlDocument();
            messageDefaultResourceDoc.LoadXml(messageDefaultResource.text);

            //Check if key exists
            var localizedValueTag = messageDefaultResourceDoc.SelectSingleNode($"//data[@name='{key}']/value");
            if (localizedValueTag != null && !string.IsNullOrEmpty(localizedValueTag?.InnerText))
            {
                return localizedValueTag.InnerText;
            }

            return null;
        }
        
        /// <summary>
        /// Removes the specified key/text from the Messages.xml file
        /// This does not remove it from the other localized files if they exist
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteMessage(string key)
        {
            //Load document
            var messageDefaultResource = Resources.Load<TextAsset>("Localization/Messages/Messages");
            var messageDefaultResourceDoc = new XmlDocument();
            messageDefaultResourceDoc.LoadXml(messageDefaultResource.text);

            //Check if key exists
            var localizedNode = messageDefaultResourceDoc.SelectSingleNode($"//data[@name='{key}']");
            if (localizedNode == null) return;

            messageDefaultResourceDoc.DocumentElement?.RemoveChild(localizedNode);
            messageDefaultResourceDoc.Save("Assets/Resources/Localization/Messages/Messages.xml");
        }
    }
}