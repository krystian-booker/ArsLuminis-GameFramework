using EventSystem.Models;
using Saving.Models;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#F9844A")]
    public class StateNode : BaseNode
    {
        [Input] public NodeLink entry;
        public EventStates eventState;

        [Output] public NodeLink stateTrue;
        [Output] public NodeLink stateFalse;
    }
}