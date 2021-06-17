using UnityEngine;
using XNode;

namespace EventSystem.Models
{
    public abstract class BaseNode : Node
    {
        [Input] public Empty entry;
        [Output] public Empty exit;

        [Tooltip("Skip camera event in sequence, helpful for debugging")]
        public bool skip;

        [HideInInspector] public bool started;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}