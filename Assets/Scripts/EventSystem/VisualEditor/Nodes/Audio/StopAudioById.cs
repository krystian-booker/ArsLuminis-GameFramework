using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Audio
{
    [NodeTint("#577590")]
    public class StopAudioById : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Id of the audio node that you want to stop")]
        public string audioNodeId;
    }
}