using System;
using System.Collections.Generic;
using XNode;

[NodeTint("#F94144")]
[CreateNodeMenu("Control Nodes/EndNode")]
public class EndNode : Node, IEventNode
{
    public bool IsSkippable { get; set; }

    public bool IsComplete { get; private set; }

    [Input] public Node entry;

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
        // No operation needed for the end node
    }

    public void Reset()
    {
        // No operation needed
    }
}
