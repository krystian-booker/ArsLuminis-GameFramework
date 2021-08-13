using EventSystem.Models;
using Input.models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    [NodeTint("#2C3A47")]
    public class ChangeInputActionMapNode : BaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;
        
        [Tooltip("Only used for documentation purposes")]
        [TextArea]
        public string description;

        [Tooltip("Action map to switch to")]
        public ActionMap actionMap;
    }
}