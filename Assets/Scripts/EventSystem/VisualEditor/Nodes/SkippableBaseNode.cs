using UnityEngine;

namespace EventSystem.VisualEditor.Nodes
{
    /// <summary>
    /// Extended BaseNode with 'skip' functionality
    /// Nodes that have conditional outputs can break logic sequences, skip should only be used
    /// in scenarios with an IN - OUT functionality
    /// </summary>
    public abstract class SkippableBaseNode : BaseNode
    {
        [Tooltip("Skip camera event in sequence, helpful for debugging")]
        public bool skip;
    }
}