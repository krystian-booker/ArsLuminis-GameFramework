using EventSystem.Models;
using XNode;

namespace EventSystem.VisualEditor.Nodes.Dialog
{
    public class DialogStartNode : Node
    {
        [Input] public Empty entry;
        [Output] public DialogNode exit;
        
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}