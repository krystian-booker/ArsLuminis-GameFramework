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

        private int _activeNodeCount;
        private bool _isUsingDefaultGraph;
        private bool _loopCurrentGraph;
        private bool _autoStartDefaultGraph;
        private NodeGraph _lastUsedGraph;

        private void Start()
        {
            ExecuteGraph();
        }

        public void ExecuteGraph(NodeGraph eventSequenceGraph = null, bool loop = false, bool autoStartDefaultGraph = false)
        {
            ResetExecutionState();

            UpdateGraphUsageSettings(eventSequenceGraph, loop, autoStartDefaultGraph);

            if (_lastUsedGraph == null)
            {
                return;
            }

            InitializeAndExecuteStartNodes();
        }

        private void ResetExecutionState()
        {
            StopAllCoroutines();
            _activeNodeCount = 0;
        }

        private void UpdateGraphUsageSettings(NodeGraph eventSequenceGraph, bool loop, bool autoStartDefaultGraph)
        {
            _isUsingDefaultGraph = eventSequenceGraph == null;
            _lastUsedGraph = eventSequenceGraph ?? defaultEventSequenceGraph;
            _loopCurrentGraph = loop;
            _autoStartDefaultGraph = autoStartDefaultGraph;
        }
         
        private void InitializeAndExecuteStartNodes()
        {
            List<StartNode> startNodes = FindStartNodes(_lastUsedGraph);
            _activeNodeCount += startNodes.Count;

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
                _activeNodeCount--;

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

            _activeNodeCount += nextNodes.Count;

            foreach (var nextNode in nextNodes)
            {
                StartCoroutine(ParseEventSequenceGraph(nextNode));
            }
        }

        private bool IsGraphExecutionComplete()
        {
            return _activeNodeCount <= 0;
        }

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