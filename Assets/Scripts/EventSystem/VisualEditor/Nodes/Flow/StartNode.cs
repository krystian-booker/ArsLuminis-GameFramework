using EventSystem.Models;

namespace EventSystem.VisualEditor.Nodes.Flow
{
    [NodeTint("#90BE6D")]
    public class StartNode : FlowNode
    {
        [Output] public NodeLink exit;
    }
}