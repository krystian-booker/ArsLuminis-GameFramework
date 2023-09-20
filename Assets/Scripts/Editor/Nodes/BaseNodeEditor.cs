using Assets.Scripts.Nodes;
using XNodeEditor;

namespace Assets.Scripts.Editor
{
    [CustomNodeEditor(typeof(ExecutableNode))]
    public class BaseNodeEditor : NodeEditor
    {
        public override int GetWidth()
        {
            return 300;
        }
    }
}
