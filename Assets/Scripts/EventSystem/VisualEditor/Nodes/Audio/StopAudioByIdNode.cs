using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Audio
{
    [NodeTint("#4a69bd")]
    public class StopAudioByIdNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Documentation purposes only"), TextArea]
        public string description;

        [Tooltip("Id of the audio node that you want to stop")]
        public string audioNodeId;
    }
}