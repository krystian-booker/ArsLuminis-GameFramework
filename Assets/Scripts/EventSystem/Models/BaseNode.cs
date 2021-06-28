using UnityEngine;
using XNode;

namespace EventSystem.Models
{
    public abstract class BaseNode : Node
    {
        [HideInInspector] public bool started;
        
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