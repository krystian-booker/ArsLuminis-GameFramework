using Assets.Scripts.Nodes;

namespace Nodes.Tools
{
    [NodeTint(51, 51, 51)]
    public class LogToConsole : ExecutableNode
    {
        public override void Execute()
        {
            UnityEngine.Debug.Log(description);
        }

        public override bool IsFinished()
        {
            return true;
        }
    }
}