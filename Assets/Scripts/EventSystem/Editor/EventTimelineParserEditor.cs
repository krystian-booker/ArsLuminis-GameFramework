using UnityEditor;
using UnityEngine;

namespace EventSystem.Editor
{
    [CustomEditor(typeof(EventTimelineParser))]
    public class EventTimelineParserEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Event sequence label at top of component
            var desc = serializedObject.FindProperty("description");
            EditorGUILayout.LabelField(desc.stringValue);
            
            var enterChildren = true;
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                switch (iterator.name)
                {
                    case "debugStep":
                        var debugger = serializedObject.FindProperty("debugger");
                        if (debugger is { boolValue: true })
                        {
                            if (GUILayout.Button("Continue ▶"))
                            {
                                iterator.boolValue = true;
                            }
                        }
                        break;
                    default:
                        EditorGUILayout.PropertyField(iterator);
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}