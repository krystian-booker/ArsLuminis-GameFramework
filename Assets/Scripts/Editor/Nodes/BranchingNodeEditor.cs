using Assets.Scripts.Nodes;
using XNodeEditor;

namespace Assets.Scripts.Editor
{
    [CustomNodeEditor(typeof(BranchingNode))]
    public class BranchingNodeEditor : NodeEditor
    {
        public override int GetWidth()
        {
            return 300;
        }
    }
}
