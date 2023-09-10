using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Nodes.Flow;
using Assets.Scripts.Interfaces;
using System.Linq;
using UnityEngine.Assertions;

namespace Assets.Scripts.Parsers
{
    public class EventSequenceParser : MonoBehaviour
    {
        [SerializeField] private NodeGraph eventSequenceGraph;

        private void Start()
        {
            Assert.IsNotNull(eventSequenceGraph, string.Format("{0}'s {1} is missing its {2}", gameObject.name, nameof(EventSequenceParser), nameof(NodeGraph)));

            List<StartNode> startNodes = FindStartNodes();
            foreach (StartNode node in startNodes)
            {
                StartCoroutine(ParseEventSequenceGraph(node));
            }
        }

        private List<StartNode> FindStartNodes()
        {
            List<StartNode> startNodes = new List<StartNode>();

            foreach (var node in eventSequenceGraph.nodes)
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
            List<IBaseNode> nextNodes;

            while (currentNode != null)
            {
                currentNode.Execute();
                yield return new WaitUntil(() => currentNode.IsFinished());

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
                    // Terminate the graph if it's a non-executable node
                    nextNodes = null;
                }

                // Start new coroutines for next nodes to execute them in parallel
                if (nextNodes != null)
                {
                    foreach (var nextNode in nextNodes)
                    {
                        StartCoroutine(ParseEventSequenceGraph(nextNode));
                    }
                }

                currentNode = null; // Since we're spawning new coroutines, we nullify the current node to terminate this one
            }
        }
    }
}