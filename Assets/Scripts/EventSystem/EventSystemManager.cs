using System.Linq;
using EventSystem.Models;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;

namespace EventSystem
{
    public class EventSystemManager : MonoBehaviour
    {
        public void ResetEventSequenceSceneGraph(EventSequenceSceneGraph eventSequenceSceneGraph)
        {
            foreach (var node in eventSequenceSceneGraph.graph.nodes.Select(node => node as BaseNode))
            {
                if (node is { })
                    node.started = false;
            }
        }
    }
}