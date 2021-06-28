using UnityEngine;

namespace EventSystem.Models
{
    /// <summary>
    /// Optional extended features for baseNode, is not always necessary for all node types
    /// </summary>
    public abstract class BaseNodeExtended : BaseNode
    {
        [Tooltip("Skip camera event in sequence, helpful for debugging")]
        public bool skip;
    }
}