using EventSystem.VisualEditor.Graphs;
using XNodeEditor;

namespace EventSystem.VisualEditor.Editor
{
    [CustomNodeGraphEditor(typeof(EventSequenceGraph))]
    public class EventSequenceGraphEditor : NodeGraphEditor 
    {
        /// <summary>
        /// Simplifies/Cleans the right click menu for xNode 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override string GetNodeMenuName(System.Type type)
        {
            return base.GetNodeMenuName(type).Replace("Event System/Visual Editor/", "");
        }
    }
}