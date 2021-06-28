using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    [NodeTint("#F9844A")]
    public class AutoSaveNode : BaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;
        
        [Tooltip("Only used for documentation purposes")]
        [TextArea]
        public string description;
    }
}
