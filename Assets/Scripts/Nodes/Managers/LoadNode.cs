using Assets.Scripts.Managers;
using Assets.Scripts.Nodes;
using System.Threading.Tasks;

namespace Nodes.Managers
{
    [NodeTint(0, 100, 100)]
    public class LoadNode : ExecutableNode
    {
        private Task loadTask = Task.CompletedTask;

        public override void Execute()
        {
            loadTask = GameManager.Instance.SaveManager.Load();
        }

        public override bool IsFinished()
        {
            return loadTask.IsCompleted;
        }
    }
}