using UnityEditor;
using XNodeEditor;
using System.Linq;

namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    [CustomNodeEditor(typeof(UpdateStateNode))]
    public class UpdateStateNodeEditor : NodeEditor
    {
        public override int GetWidth()
        {
            return 300;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;

                EditorGUIUtility.labelWidth = 120;
                NodeEditorGUILayout.PropertyField(iterator);
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (var dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}