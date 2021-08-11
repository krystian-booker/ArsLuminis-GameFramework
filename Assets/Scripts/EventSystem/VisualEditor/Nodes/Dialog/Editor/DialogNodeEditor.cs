using System.Linq;
using EventSystem.VisualEditor.Nodes.Dialog.Models;
using Tools;
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
                var trackedOption = dialogNode.optionsKeyTracker.FirstOrDefault(x => x.Equals(option.key));
                if (trackedOption == null || string.IsNullOrEmpty(trackedOption))
                {
                    option.text = "";
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

        public override int GetWidth()
        {
            return 500;
        }
    }
}