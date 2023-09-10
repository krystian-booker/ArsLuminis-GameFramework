using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Assets.Scripts.Nodes
{
    public abstract class ExecutableNode : Node, IExecutableNode
    {
        [Input]
        public NodePort entry;

        [Output]
        public NodePort exit;

        [Tooltip("Documentation purposes only")]
        [TextArea]
        public string description;

        public bool skip;

        public NodePort Entry
        {
            get { return entry; }
        }

        public NodePort Exit
        {
            get { return exit; }
        }

        public bool Skip
        {
            get { return skip; }
            set { skip = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public abstract void Execute();

        public abstract bool IsFinished();

        public override object GetValue(NodePort port)
        {
            return null;
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

        public List<IBaseNode> GetConnectedInputs()
        {
            throw new System.NotImplementedException();
        }
    }
}