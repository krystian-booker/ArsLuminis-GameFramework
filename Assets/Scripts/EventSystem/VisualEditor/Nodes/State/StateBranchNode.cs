using EventSystem.Models;
using Saving.Models;

namespace EventSystem.VisualEditor.Nodes.State
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#3d5163")]
    public class StateBranchNode : BaseNode
    {
        [Input] public NodeLink entry;
        // public EventStates eventState;

        [Output] public NodeLink stateTrue;
        [Output] public NodeLink stateFalse;
    }
}