using UnityEngine;
using XNode;

namespace Assets.Scripts.Models.Graphs
{
    public class EventSequenceSceneGraph : SceneGraph<EventSequenceGraph>
    {
        [TextArea, Tooltip("No code usage")] public string description;
    }
}
