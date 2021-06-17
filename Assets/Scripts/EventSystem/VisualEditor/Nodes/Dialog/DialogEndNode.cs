using EventSystem.Models;
using XNode;

namespace EventSystem.VisualEditor.Nodes.Dialog
{
    public class DialogEndNode: Node
    {
        [Input] public DialogNode entry;
        [Output] public Empty exit;
        
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}