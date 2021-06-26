using EventSystem.Models;

namespace EventSystem.VisualEditor.Nodes.Flow
{
    [NodeTint("#F94144")]
    public class EndNode : FlowNode
    {
        [Input] public NodeLink entry;
    }
}