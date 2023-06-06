using System.Collections.Generic;
using XNode;

public interface IEventNode
{
    // Whether or not the node has finished executing
    bool IsComplete { get; }

    // Property to decide whether the node is skippable or not
    bool IsSkippable { get; set; }

    // Initialize the node with any required data
    void Initialize(Dictionary<string, object> data);

    // Start executing the node's action
    void Execute();

    // Reset the node to its initial state
    void Reset();

    // This method is called each update while the node is executing
    void OnUpdate();
}
