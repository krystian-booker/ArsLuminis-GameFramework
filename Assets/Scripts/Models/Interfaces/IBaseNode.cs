using System.Collections.Generic;
using XNode;

namespace Assets.Scripts.Interfaces
{
    public interface IBaseNode
    {
        void Execute();
        bool IsFinished();
        List<IBaseNode> GetConnectedInputs();
        List<IBaseNode> GetConnectedOutputs();

    }
}
