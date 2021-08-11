using UnityEngine;
using XNode;

namespace EventSystem.VisualEditor.Nodes
{
    public abstract class BaseNode : Node
    {
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