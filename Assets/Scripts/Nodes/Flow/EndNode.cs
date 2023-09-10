using Assets.Scripts.Nodes;
using UnityEngine;
using XNode;

namespace Nodes.Flow
{
    [NodeTint(158, 36, 36)]
    public class EndNode : FlowNode
    {
        [Input]
        public NodePort Entry;

        public override void Execute()
        {
            // Do nothing
        }

        public override bool IsFinished()
        {
            return true;
        }
    }
}
