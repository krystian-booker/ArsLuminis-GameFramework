using EventSystem.Models;
using Saving.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#F9844A")]
    public class UpdateStateNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;
        
        [LabelWidth(100), Tooltip("Event state to be updated")]
        public EventStates eventState;
        
        [LabelWidth(100), Tooltip("The eventState will be set to this value")]
        public bool stateComplete = false;
    }
}