using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Animation
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#40407a")]
    public class PlayAnimationNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Documentation purposes only")] [TextArea]
        public string description;

        [Tooltip("The event sequence will only continue when animation event runs. If disabled, the event sequence will continue instantly.")]
        public bool continueOnAnimationEvent = true;

        [Tooltip("GameObject you want the animation to run on.")]
        public GameObject animationTarget;

        [Tooltip("Trigger on animation controller that will be hit when node is ran")]
        public string animationTrigger;
    }
}