using EventSystem.VisualEditor.Graphs;
using XNodeEditor;

namespace EventSystem.VisualEditor.Editor
{
    [CustomNodeGraphEditor(typeof(EventSequenceGraph))]
    public class EventSequenceGraphEditor : NodeGraphEditor 
    {
        public override string GetNodeMenuName(System.Type type)
        {
            return base.GetNodeMenuName(type).Replace("Event System/Visual Editor/", "");
        }
    }
}