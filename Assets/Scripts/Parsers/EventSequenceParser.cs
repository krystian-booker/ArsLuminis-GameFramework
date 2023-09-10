using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Nodes.Flow;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Parsers
{
    public class EventSequenceParser : MonoBehaviour
    {
        [Tooltip("Default EventSequenceGraph, not required")]
        [SerializeField] private NodeGraph defaultEventSequenceGraph;

        [Tooltip("Should the default EventSequenceGraph loop?")]
        [SerializeField] private bool loopDefaultEventSequence;

        private int activeNodeCount;
        private bool isUsingDefaultGraph = true;
        private bool loopCurrentGraph;
        private NodeGraph lastUsedGraph;

        private void Start()
        {
            ExecuteGraph();
        }

        public void ExecuteGraph(NodeGraph eventSequenceGraph = null, bool loop = false)
        {
            ResetExecutionState();

            UpdateGraphUsageSettings(eventSequenceGraph, loop);

            if (lastUsedGraph == null)
            {
                return;
            }

            InitializeAndExecuteStartNodes();
        }

        private void ResetExecutionState()
        {
            StopAllCoroutines();
            activeNodeCount = 0;
        }

        private void UpdateGraphUsageSettings(NodeGraph eventSequenceGraph, bool loop)
        {
            isUsingDefaultGraph = eventSequenceGraph == null;
            lastUsedGraph = eventSequenceGraph ?? defaultEventSequenceGraph;
            loopCurrentGraph = loop;
        }

        private void InitializeAndExecuteStartNodes()
        {
            List<StartNode> startNodes = FindStartNodes(lastUsedGraph);
            activeNodeCount += startNodes.Count;

            foreach (StartNode node in startNodes)
            {
                StartCoroutine(ParseEventSequenceGraph(node));
            }
        }

        private List<StartNode> FindStartNodes(NodeGraph graph)
        {
            List<StartNode> startNodes = new List<StartNode>();
            foreach (var node in graph.nodes)
            {
                if (node is StartNode startNode && !startNode.Skip)
                {
                    startNodes.Add(startNode);
                }
            }
            return startNodes;
        }

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
                activeNodeCount--;

                if (IsGraphExecutionComplete())
                {
                    HandleGraphCompletion();
                }
            }
        }

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
            else
            {
                nextNodes = null;
            }

            return nextNodes;
        }

        private void ProcessNextNodes(List<IBaseNode> nextNodes)
        {
            if (nextNodes == null) return;

            activeNodeCount += nextNodes.Count;

            foreach (var nextNode in nextNodes)
            {
                StartCoroutine(ParseEventSequenceGraph(nextNode));
            }
        }

        private bool IsGraphExecutionComplete()
        {
            return activeNodeCount <= 0;
        }

        private void HandleGraphCompletion()
        {
            if (!isUsingDefaultGraph)
            {
                if (loopCurrentGraph)
                {
                    // Pass in the last used graph and set loop to true
                    ExecuteGraph(lastUsedGraph, true);
                }
                else
                {
                    // Use the default graph and its loop setting
                    isUsingDefaultGraph = true;
                    ExecuteGraph(defaultEventSequenceGraph, loopDefaultEventSequence);
                }
            }
            else if (loopDefaultEventSequence)
            {
                // Use the default graph and its loop setting
                ExecuteGraph(defaultEventSequenceGraph, loopDefaultEventSequence);
            } else
            {
                // end
            }
        }
    }
}