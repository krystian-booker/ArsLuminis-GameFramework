using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nodes.Flow;
using Assets.Scripts.Models.Interfaces;
using Assets.Scripts.Models.Graphs;
using Assets.Scripts.Nodes;

namespace Assets.Scripts.Parsers
{
    public class EventSequenceParser : MonoBehaviour
    {
        [Tooltip("Default EventSequenceGraph, not required")]
        [SerializeField] private EventSequenceSceneGraph defaultEventSequenceGraph;

        [Tooltip("Should the default EventSequenceGraph loop?")]
        [SerializeField] private bool loopDefaultEventSequence;

        private int _activeNodeCount;
        private bool _isUsingDefaultGraph;
        private bool _loopCurrentGraph;
        private bool _autoStartDefaultGraph;
        private EventSequenceSceneGraph _lastUsedGraph;

        private void Start()
        {
            ExecuteGraph();
        }

        public EventSequenceSceneGraph GetDefaultEventSequenceGraph()
        {
            return defaultEventSequenceGraph;
        }

        /// <summary>
        /// Executes the event sequence graph, managing the internal state and control flow as needed.
        /// The method resets the execution state, updates the graph usage settings based on the provided parameters,
        /// and then initializes and executes the start nodes of the last used graph.
        /// </summary>
        /// <param name="eventSequenceGraph">The event sequence graph to execute. If null, the last used graph or default graph will be used.</param>
        /// <param name="loop">Flag indicating whether the current graph should loop after execution is complete.</param>
        /// <param name="autoStartDefaultGraph">Flag indicating whether the default graph should automatically start after the current graph finishes.</param>
        public void ExecuteGraph(EventSequenceSceneGraph eventSequenceGraph = null, bool loop = false, bool autoStartDefaultGraph = false)
        {
            ResetExecutionState();

            UpdateGraphUsageSettings(eventSequenceGraph, loop, autoStartDefaultGraph);

            if (_lastUsedGraph == null)
            {
                return;
            }

            InitializeAndExecuteStartNodes();
        }

        /// <summary>
        /// Resets the state related to the execution of the event sequence graph.
        /// It stops all currently running coroutines and resets the count of active nodes to zero.
        /// </summary>
        private void ResetExecutionState()
        {
            StopAllCoroutines();
            _activeNodeCount = 0;
        }

        /// <summary>
        /// Updates the internal settings related to the usage of event sequence graphs.
        /// It sets various flags such as whether the default graph is in use,
        /// what the last used graph was, whether to loop the current graph, 
        /// and whether to automatically start the default graph.
        /// </summary>
        /// <param name="eventSequenceGraph">The event sequence graph that will be set as the last used graph.</param>
        /// <param name="loop">Flag indicating whether the current graph should loop after execution is complete.</param>
        /// <param name="autoStartDefaultGraph">Flag indicating whether the default graph should automatically start after the current graph finishes.</param>
        private void UpdateGraphUsageSettings(EventSequenceSceneGraph eventSequenceGraph, bool loop, bool autoStartDefaultGraph)
        {
            _isUsingDefaultGraph = eventSequenceGraph == null;
            _lastUsedGraph = eventSequenceGraph ?? defaultEventSequenceGraph;
            _loopCurrentGraph = loop;
            _autoStartDefaultGraph = autoStartDefaultGraph;
        }

        /// <summary>
        /// Initializes and executes the sequence of events starting from the "StartNode" nodes found in the last used graph.
        /// It locates all the "StartNode" instances in the '_lastUsedGraph' and increments the '_activeNodeCount' accordingly.
        /// Then, it starts coroutines for parsing and executing the event sequences stemming from each "StartNode".
        /// </summary>
        private void InitializeAndExecuteStartNodes()
        {
            List<StartNode> startNodes = FindStartNodes(_lastUsedGraph);
            _activeNodeCount += startNodes.Count;

            foreach (StartNode node in startNodes)
            {
                StartCoroutine(ParseEventSequenceGraph(node));
            }
        }

        /// <summary>
        /// Finds and returns a list of "StartNode" instances in a given EventSequenceSceneGraph.
        /// Only includes "StartNode" instances that have their 'Skip' property set to false.
        /// </summary>
        /// <param name="sceneGraph">The EventSequenceSceneGraph containing nodes to search for "StartNode" instances.</param>
        /// <returns>
        /// A list of "StartNode" instances found in the given scene graph that should not be skipped.
        /// Returns an empty list if no such nodes are found.
        /// </returns>
        private List<StartNode> FindStartNodes(EventSequenceSceneGraph sceneGraph)
        {
            List<StartNode> startNodes = new List<StartNode>();
            foreach (var node in sceneGraph.graph.nodes)
            {
                if (node is StartNode startNode && !startNode.Skip)
                {
                    startNodes.Add(startNode);
                }
            }
            return startNodes;
        }

        /// <summary>
        /// Parses and executes an event sequence graph starting from the given node, moving along connected output nodes.
        /// The method uses coroutines to wait for the execution of each node to complete before moving to the next node(s).
        /// It also updates '_activeNodeCount' and checks for graph execution completion.
        /// </summary>
        /// <param name="node">The initial IBaseNode from which to start the event sequence graph traversal.</param>
        /// <returns>
        /// An IEnumerator that can be used to control the coroutine from the calling method.
        /// </returns>
        private IEnumerator ParseEventSequenceGraph(IBaseNode node)
        {
            IBaseNode currentNode = node;

            while (currentNode != null)
            {
                currentNode.Execute();
                yield return new WaitUntil(() => currentNode.IsFinished());

                var nextNodes = GetConnectedOutputs(currentNode);
                ProcessNextNodes(nextNodes);

                currentNode = null;
                _activeNodeCount--;

                if (IsGraphExecutionComplete())
                {
                    HandleGraphCompletion();
                }
            }
        }

        /// <summary>
        /// Retrieves a list of connected output nodes for a given node in the event sequence graph.
        /// The type of the current node determines which method will be called to obtain connected output nodes.
        /// This method supports 'StartNode', 'IExecutableNode', and 'BranchingNode' types.
        /// </summary>
        /// <param name="currentNode">The node for which to find connected output nodes.</param>
        /// <returns>
        /// A list of IBaseNode objects that are connected to the output of the current node.
        /// Returns null if the type of the node is not supported.
        /// </returns>
        private static List<IBaseNode> GetConnectedOutputs(IBaseNode currentNode)
        {
            List<IBaseNode> nextNodes;
            if (currentNode is StartNode startNode)
            {
                nextNodes = startNode.GetConnectedOutputs();
            }
            else if (currentNode is IExecutableNode executableNode)
            {
                nextNodes = executableNode.GetConnectedOutputs();
            }
            else if (currentNode is BranchingNode branchingNode)
            {
                nextNodes = branchingNode.GetConnectedOutputs();
            }
            else
            {
                nextNodes = null;
            }

            return nextNodes;
        }

        /// <summary>
        /// Processes a list of next nodes in the event sequence graph by initiating their execution.
        /// The method increases '_activeNodeCount' based on the number of nodes to be processed and 
        /// launches a coroutine to parse each node's sequence in the graph.
        /// </summary>
        /// <param name="nextNodes">A list of IBaseNode instances that represent the next nodes to be processed.</param>
        private void ProcessNextNodes(List<IBaseNode> nextNodes)
        {
            if (nextNodes == null) return;

            _activeNodeCount += nextNodes.Count;

            foreach (var nextNode in nextNodes)
            {
                StartCoroutine(ParseEventSequenceGraph(nextNode));
            }
        }

        /// <summary>
        /// Checks whether the execution of the event sequence graph is complete.
        /// The method inspects the '_activeNodeCount' field to determine if there are any active nodes remaining.
        /// </summary>
        /// <returns>
        /// Returns 'true' if the execution of the graph is complete, indicated by '_activeNodeCount' being zero or negative.
        /// Returns 'false' otherwise.
        /// </returns>
        private bool IsGraphExecutionComplete()
        {
            return _activeNodeCount <= 0;
        }

        /// <summary>
        /// Handles the completion of the current event sequence graph's execution.
        /// Based on various flags and settings, it either loops the current graph,
        /// switches to the default graph, or initiates another execution as appropriate.
        /// </summary>
        private void HandleGraphCompletion()
        {
            if (_isUsingDefaultGraph)
            {
                if (loopDefaultEventSequence)
                {
                    // Use the default graph and its loop setting
                    ExecuteGraph(defaultEventSequenceGraph, loopDefaultEventSequence);
                }
            }
            else
            {
                if (_loopCurrentGraph)
                {
                    // Pass in the last used graph and set loop to true
                    ExecuteGraph(_lastUsedGraph, true);
                }
                else if (_autoStartDefaultGraph)
                {
                    ExecuteGraph();
                }
            }
        }
    }
}