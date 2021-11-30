using UnityEngine;
using XNode;

namespace EventSystem.VisualEditor.Graphs
{
    /// <summary>
    /// Required to be defined by xNode
    /// </summary>
    public class EventSequenceGraph : SceneGraph<EventSequenceNodeGraph>
    {
        [TextArea, Tooltip("No code usage")] public string description;
    }
}