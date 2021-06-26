using UnityEngine;
using XNode;

namespace EventSystem.Models
{
    public abstract class BaseNode : Node
    {
        [HideInInspector] public bool started;

        [Tooltip("Skip camera event in sequence, helpful for debugging")]
        public bool skip;
        
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}