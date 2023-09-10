using Assets.Scripts.Nodes;
using UnityEngine;
using XNode;

namespace Nodes.Flow
{
    [NodeTint(0, 102, 51)]
    public class StartNode : FlowNode
    {
        [Output]
        public NodePort Exit;

        [Tooltip("Skips entire branch stemming from this Start Node")]
        public bool Skip;

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
