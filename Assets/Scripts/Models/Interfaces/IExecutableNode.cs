using XNode;

namespace Assets.Scripts.Models.Interfaces
{
    public interface IExecutableNode : IBaseNode
    {
        NodePort Entry { get; }
        NodePort Exit { get; }
        bool Skip { get; set; }
    }
}