using UnityEngine;
using XNode;

namespace EventSystem.VisualEditor.Graphs
{
    /// <summary>
    /// Required to be defined by xNode
    /// </summary>
    public class EventSequenceSceneGraph : SceneGraph<EventSequenceGraph>
    {
        [TextArea, Tooltip("No code usage")] public string description;
    }
}