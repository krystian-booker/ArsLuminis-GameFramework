using System.Linq;
using EventSystem.VisualEditor.Nodes.Dialog.Models;
using Tools;
using UnityEditor;
using XNode;
using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.Dialog.Editor
{
    [CustomNodeEditor(typeof(DialogNode))]
    public class DialogNodeEditor : NodeEditor
    {
        public DialogNodeEditor()
        {
            //Builtin action on xNode
            onUpdateNode += UpdateLocalizationFileOption;
            onUpdateNode += ValidateNodeStates;
        }

        /// <summary>
        /// Node Width
        /// </summary>
        /// <returns></returns>
        public override int GetWidth()
        {
            return 400;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();
            
            string[] excludes = { "m_Script", "graph", "position", "ports" };

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren)) {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                switch (iterator.name)
                {
                    case "exit":
                        if (!target.DynamicPorts.Any())
                        {
                            NodeEditorGUILayout.PropertyField(iterator);
                        }
                        break;
                    case "displayTime":
                        var displayForNTime = serializedObject.FindProperty("displayForNTime");
                        if (displayForNTime is { boolValue: true })
                        {
                            NodeEditorGUILayout.PropertyField(iterator);
                        }
                        else
                        {
                            //Clear previous value
                            iterator.intValue = 0;
                        }
                        break;
                    case "timePerCharacter":
                        var customTimePerCharacter = serializedObject.FindProperty("customTimePerCharacter");
                        if (customTimePerCharacter is { boolValue: true })
                        {
                            NodeEditorGUILayout.PropertyField(iterator);
                        }
                        else
                        {
                            //Clear previous value
                            iterator.intValue = 0;
                        }
                        break;
                    case "dialogPositionX":
                    case "dialogPositionY":
                        var customDialogPosition = serializedObject.FindProperty("customDialogPosition");
                        if (customDialogPosition is { boolValue: true })
                        {
                            NodeEditorGUILayout.PropertyField(iterator);
                        }
                        else
                        {
                            //Clear previous value
                            iterator.intValue = -1;
                        }
                        break;
                    default:
                        EditorGUIUtility.labelWidth = 165;
                        NodeEditorGUILayout.PropertyField(iterator);
                        break;
                }
            }
            
            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (var dynamicPort in target.DynamicPorts) {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void ValidateNodeStates(Node node)
        {
            var dialogNode = node as DialogNode;
            if (dialogNode == null) return;

            if (dialogNode.DynamicPorts.Any())
            {
                var output = node.Outputs.FirstOrDefault();
                output?.ClearConnections();
            }
        }
        
        /// <summary>
        /// Due to a bug in xNode, when adding a new element to our Options list, we need to remove the new element and
        /// add a new value again.
        /// Once this has been completed we check if any text needs to be updated in the Localization files
        /// </summary>
        /// <param name="node"></param>
        private void UpdateLocalizationFileOption(Node node)
        {
            var dialogNode = node as DialogNode;
            if (dialogNode == null) return;

            //Bug in xNode that when adding a new element it will clone the previous object instead of a new instance.
            //To resolve this we remove the last object added (the clone) and add a new instance.
            if (dialogNode.nCount < dialogNode.options.Count)
            {
                dialogNode.options.Remove(dialogNode.options.LastOrDefault());
                dialogNode.options.Add(new DialogOption());
            }
            dialogNode.nCount = dialogNode.options.Count;

            //Clear text if key has changed
            foreach (var option in dialogNode.options)
            {
                if(string.IsNullOrEmpty(option.key))
                    continue;
                
                var trackedOption = dialogNode.optionsKeyTracker.FirstOrDefault(x => x.Equals(option.key));
                if (trackedOption == null || string.IsNullOrEmpty(trackedOption))
                {
                    option.text = string.Empty;
                }
            }

            //Clear tracked keys and regenerate list once validation is complete
            dialogNode.optionsKeyTracker.Clear();
            foreach (var option in dialogNode.options)
            {
                dialogNode.optionsKeyTracker.Add(option.key);
            }
            
            //Update localization and get messages for keys
            Utilities.BulkOptionUpdateMessages(dialogNode.options);
        }
    }
}