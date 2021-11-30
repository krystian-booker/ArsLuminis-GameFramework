using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Flow
{
    [NodeTint("#AD39B8")]
    public class WaitNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [SerializeField] public float delayTime;

        [Tooltip("Documentation purposes only")] [TextArea]
        public string description;
    }
}