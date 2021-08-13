using EventSystem.VisualEditor.Nodes;
using XNodeEditor;

namespace EventSystem.VisualEditor.Editor
{
    [CustomNodeEditor(typeof(BaseNode))]
    public class BaseNodeEditor : NodeEditor
    {
        public override int GetWidth() {
            return 300;
        }
    }
}