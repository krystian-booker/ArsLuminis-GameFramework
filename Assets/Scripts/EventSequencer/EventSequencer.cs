using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XNode;

public class EventSequencer : MonoBehaviour
{
    // The xNode graph for this sequencer
    [SerializeField] private EventGraph nodeGraph;

    // The current node being executed
    private IEventNode currentNode;

    private void Start()
    {
        // Find the start node in the graph
        var startNode = (IEventNode)nodeGraph.nodes.Find(n => n is StartNode);

        // Initialize and start executing the start node
        startNode.Initialize(new Dictionary<string, object>());
        startNode.Execute();

        // Set the current node to the start node
        currentNode = startNode;
    }

    private void Update()
    {
        if (currentNode != null)
        {
            // If current node is not complete, call its OnUpdate method
            if (!currentNode.IsComplete)
            {
                currentNode.OnUpdate();
            }

            // If node is complete or skippable, move to the next node
            if (currentNode.IsComplete || currentNode.IsSkippable)
            {
                GoToNextNode();
            }
        }
    }

    private void GoToNextNode()
    {
        //if (currentNode.NextNodes.Count > 0)
        //{
        //    // Initialize and start executing the next node(s)
        //    foreach (var nextNode in currentNode.NextNodes)
        //    {
        //        nextNode.Initialize(new Dictionary<string, object>());
        //        nextNode.Execute();
        //        currentNode = nextNode;
        //    }
        //}
        //else
        //{
        //    // If the current node is complete and has no next nodes, the sequence is finished
        //    currentNode = null;
        //}
    }
}
