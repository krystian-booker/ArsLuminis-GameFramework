using EventSystem.Models;
using SystemInput;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    [NodeTint("#3d5163")]
    public class ChangeInputActionMapNode : BaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Only used for documentation purposes")] [TextArea]
        public string description;

        [Tooltip("Action map to switch to")] public ActionMap actionMap;
    }
}