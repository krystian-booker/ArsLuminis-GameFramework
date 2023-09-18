using Assets.Scripts.Models.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Assets.Scripts.Nodes
{
    public abstract class FlowNode : Node, IBaseNode
    {
        [Tooltip("Documentation purposes only")]
        [TextArea]
        public string Description;

        public abstract void Execute();

        public abstract bool IsFinished();

        public override object GetValue(NodePort port)
        {
            return null;
        }

        public List<IBaseNode> GetConnectedInputs()
        {
            throw new System.NotImplementedException();
        }

        public List<IBaseNode> GetConnectedOutputs()
        {
            var outputs = new List<IBaseNode>();
            foreach (NodePort outputPort in Outputs)
            {
                foreach (NodePort connectedPort in outputPort.GetConnections())
                {
                    outputs.Add(connectedPort.node as IBaseNode);
                }
            }

            return outputs;
        }
    }
}