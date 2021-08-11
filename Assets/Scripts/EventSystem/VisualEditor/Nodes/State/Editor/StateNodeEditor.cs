using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    [CustomNodeEditor(typeof(StateNode))]
    public class StateNodeEditor : NodeEditor
    {
        public override int GetWidth() {
            return 300;
        }
    }
}