using UnityEngine;
using XNode;

namespace EventSystem.Models
{
    public abstract class BaseNode : Node
    {
        [HideInInspector] public bool started;

        [Tooltip("Skip camera event in sequence, helpful for debugging")]
        public bool skip;
        
        /// <summary>
        /// Required by xNode
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}