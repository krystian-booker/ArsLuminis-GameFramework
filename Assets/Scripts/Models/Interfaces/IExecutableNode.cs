using System.Collections.Generic;
using XNode;

namespace Assets.Scripts.Interfaces
{
    public interface IExecutableNode : IBaseNode
    {
        NodePort Entry { get; }
        NodePort Exit { get; }
        bool Skip { get; set; }
    }
}