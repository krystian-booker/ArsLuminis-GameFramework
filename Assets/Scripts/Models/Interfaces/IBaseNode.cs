using System.Collections.Generic;

namespace Assets.Scripts.Models.Interfaces
{
    public interface IBaseNode
    {
        void Execute();
        bool IsFinished();
        List<IBaseNode> GetConnectedInputs();
        List<IBaseNode> GetConnectedOutputs();

    }
}
