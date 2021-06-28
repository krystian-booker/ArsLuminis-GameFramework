using EventSystem.VisualEditor.Nodes.Actions;
using EventSystem.VisualEditor.Nodes.State;
using XNodeEditor;

namespace EventSystem.VisualEditor.Editor
{
    [CustomNodeEditor(typeof(StateNode))]
    public class StateNodeEditor : NodeEditor
    {
        public override int GetWidth() {
            return 300;
        }
    }
}