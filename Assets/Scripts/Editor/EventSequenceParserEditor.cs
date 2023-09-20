#if UNITY_EDITOR
using Assets.Scripts.Models.Graphs;
using Assets.Scripts.Parsers;
using UnityEditor;

[CustomEditor(typeof(EventSequenceParser))]
public class EventSequenceParserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Fetch the target script that we are inspecting
        EventSequenceParser parser = (EventSequenceParser)target;

        // Draw the default UI for all other fields
        serializedObject.Update();

        // Iterate through each property and draw them, except the one we want to customize
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            // Skip the 'defaultEventSequenceGraph' field to handle it separately
            if (property.name == "defaultEventSequenceGraph")
            {
                continue;
            }

            EditorGUILayout.PropertyField(property, true);
        }

        // Handle 'defaultEventSequenceGraph' field separately
        SerializedProperty graphProperty = serializedObject.FindProperty("defaultEventSequenceGraph");
        EventSequenceSceneGraph graph = parser.GetDefaultEventSequenceGraph();
        string description = (graph != null) ? graph.description : "None";

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Graph (Description: " + description + ")");
        graphProperty.objectReferenceValue = EditorGUILayout.ObjectField(graphProperty.objectReferenceValue, typeof(EventSequenceSceneGraph), true);
        EditorGUILayout.EndHorizontal();

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}
#endif