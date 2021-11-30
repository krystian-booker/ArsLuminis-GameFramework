using UnityEditor;
using XNodeEditor;
using System.Linq;
using Tools;
using UnityEngine;


namespace EventSystem.VisualEditor.Nodes.Locomotion.Editor
{
    [CustomNodeEditor(typeof(ObjectMovementNode))]
    public class ObjectMovementNodeEditor : NodeEditor
    {
        /// Node Width
        public override int GetWidth()
        {
            return 350;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();
            string[] excludes = {"m_Script", "graph", "position", "ports"};

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                switch (iterator.name)
                {
                    case "targetPosition":
                        EditorGUILayout.BeginHorizontal();
                        EditorGUIUtility.labelWidth = 120;
                        NodeEditorGUILayout.PropertyField(iterator);
                        if (GUILayout.Button("+", GUILayout.Width(30)))
                        {
                            GenerateTargetPosition();
                        }

                        EditorGUILayout.EndHorizontal();
                        break;
                    case "startingPosition":
                        EditorGUILayout.BeginHorizontal();
                        EditorGUIUtility.labelWidth = 120;
                        NodeEditorGUILayout.PropertyField(iterator);
                        if (GUILayout.Button("+", GUILayout.Width(30)))
                        {
                            GenerateStartingPosition();
                        }

                        EditorGUILayout.EndHorizontal();
                        break;
                    default:
                        EditorGUIUtility.labelWidth = 120;
                        NodeEditorGUILayout.PropertyField(iterator);
                        break;
                }
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (var dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void GenerateTargetPosition()
        {
            var positionTargetGameObject = Resources.Load<GameObject>("Prefabs/editorTools/YellowTarget");
            if (positionTargetGameObject == null) return;

            //Get node values
            var shortName = serializedObject.FindProperty("shortName");
            var targetPosition = serializedObject.FindProperty("targetPosition");

            //Assign object back to self
            var instantiatedTarget = Utilities.InstantiateObject(positionTargetGameObject);
            instantiatedTarget.name =
                string.IsNullOrEmpty(shortName.stringValue) ? "TargetPosition" : $"{shortName}TargetPosition";
            targetPosition.objectReferenceValue = instantiatedTarget;
        }

        private void GenerateStartingPosition()
        {
            var positionTargetGameObject = Resources.Load<GameObject>("Prefabs/editorTools/GreenTarget");
            if (positionTargetGameObject == null) return;

            //Get node values
            var shortName = serializedObject.FindProperty("shortName");
            var startingPosition = serializedObject.FindProperty("startingPosition");

            //Assign object back to self
            var instantiatedTarget = Utilities.InstantiateObject(positionTargetGameObject);
            instantiatedTarget.name =
                string.IsNullOrEmpty(shortName.stringValue) ? "StartingPosition" : $"{shortName}StartingPosition";
            startingPosition.objectReferenceValue = instantiatedTarget;
        }
    }
}