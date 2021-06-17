using EventSystem.Models;
using EventSystem.VisualEditor.Nodes.Flow;
using UnityEngine;
using XNodeEditor;

namespace EventSystem.VisualEditor.Editor
{
    [CustomNodeEditor(typeof(FlowNode))]
    public class FlowNodeEditor : NodeEditor
    {
        public override Color GetTint()
        {
            if (target.GetType() == typeof(StartNode))
            {
                return new Color(0.08f, 1f, 0.5f);
            }
            return target.GetType() == typeof(EndNode) ? new Color(1f, 0.36f, 0.25f) : Color.yellow;
        }
    }
}