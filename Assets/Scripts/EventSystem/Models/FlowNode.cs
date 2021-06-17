using UnityEngine;
using XNode;

namespace EventSystem.Models
{
    public abstract class FlowNode: Node
    {
        [HideInInspector]
        public bool started;

        public override object GetValue(NodePort port) {
            return null;
        }
    }
}