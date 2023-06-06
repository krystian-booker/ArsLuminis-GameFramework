using System.Collections.Generic;
using XNode;

[NodeTint("#90BE6D")]
[CreateNodeMenu("Control Nodes/StartNode")]
public class StartNode : Node, IEventNode
{
    public bool IsSkippable { get; set; }

    public bool IsComplete { get; private set; }

    [Output] public Node exit;

    public string input;
    public string output;

    public void Initialize(Dictionary<string, object> parameters)
    {
        // Implementation based on your game's requirements
    }

    public void Execute()
    {
        // Mark this node as complete as soon as it executes
        IsComplete = true;
    }

    public void OnUpdate()
    {
        // No operation needed for the start node
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "exit")
        {
            return exit;
        }
        return null;
    }

    public void Reset()
    {
        // No operation needed
    }
}
