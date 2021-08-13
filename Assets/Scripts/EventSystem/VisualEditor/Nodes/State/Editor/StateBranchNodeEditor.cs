using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    [CustomNodeEditor(typeof(StateBranchNode))]
    public class StateBranchNodeEditor : NodeEditor
    {
        public override int GetWidth() {
            return 300;
        }
    }
}