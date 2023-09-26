using Assets.Scripts.Managers;
using Assets.Scripts.Nodes;

namespace Nodes.Managers
{
    [NodeTint(0, 100, 100)]
    public class SaveNode : ExecutableNode
    {
        public override void Execute()
        {
            GameManager.Instance.SaveManager.SaveGame();
        }

        public override bool IsFinished()
        {
            return true;
        }
    }
}