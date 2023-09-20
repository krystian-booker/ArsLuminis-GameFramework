using Assets.Scripts.Models.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Assets.Scripts.Nodes
{
    public abstract class BranchingNode : Node, IBaseNode
    {
        [Input]
        public NodePort entry;

        [Output]
        public Dictionary<string, NodePort> customExits = new Dictionary<string, NodePort>();


        [Tooltip("Documentation purposes only")]
        [TextArea]
        public string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public abstract void Execute();

        public abstract bool IsFinished();

        public abstract List<IBaseNode> GetConnectedOutputs();

        public override object GetValue(NodePort port)
        {
            return null;
        }

        public void AddExit(string exitName)
        {
            if (customExits.ContainsKey(exitName))
            {
                Debug.LogWarning($"Exit with name {exitName} already exists.");
                return;
            }

            NodePort newPort = AddDynamicOutput(typeof(NodePort), fieldName: exitName);
            customExits[exitName] = newPort;
        }

        public List<IBaseNode> GetConnectedInputs()
        {
            throw new System.NotImplementedException();
        }
    }
}