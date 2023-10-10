using Assets.Scripts.Managers;
using Assets.Scripts.Nodes;
using System.Threading.Tasks;

namespace Nodes.Managers
{
    [NodeTint(0, 100, 100)]
    public class SaveNode : ExecutableNode
    {
        private Task loadTask = Task.CompletedTask;

        public override void Execute()
        {
            loadTask = GameManager.Instance.SaveManager.Save();
        }

        public override bool IsFinished()
        {
            return loadTask.IsCompleted;
        }
    }
}