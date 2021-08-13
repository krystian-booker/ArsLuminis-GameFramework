using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    [NodeTint("#2C3A47")]
    public class AutoSaveNode : BaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;
        
        [Tooltip("Only used for documentation purposes")]
        [TextArea]
        public string description;
    }
}
