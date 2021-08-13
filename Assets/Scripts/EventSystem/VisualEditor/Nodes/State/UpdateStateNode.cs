using EventSystem.Models;
using Saving.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#3d5163")]
    public class UpdateStateNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;
        
        [Tooltip("Event state to be updated")]
        public EventStates eventState;
        
        [Tooltip("The eventState will be set to this value")]
        public bool stateComplete = false;
    }
}