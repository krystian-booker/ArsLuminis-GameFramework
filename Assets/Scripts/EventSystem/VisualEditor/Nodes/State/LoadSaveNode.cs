using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    [NodeTint("#3d5163")]
    public class LoadSaveNode : BaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;
        
        [Tooltip("Only used for documentation purposes"), TextArea]
        public string description;

        public string saveFileName;
    }
}