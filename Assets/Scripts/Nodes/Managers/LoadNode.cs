using Assets.Scripts.Managers;
using Assets.Scripts.Nodes;

namespace Nodes.Managers
{
    [NodeTint(0, 100, 100)]
    public class LoadNode : ExecutableNode
    {
        public override void Execute()
        {
            GameManager.Instance.SaveManager.LoadGame("");
        }

        public override bool IsFinished()
        {
            return true;
        }
    }
}