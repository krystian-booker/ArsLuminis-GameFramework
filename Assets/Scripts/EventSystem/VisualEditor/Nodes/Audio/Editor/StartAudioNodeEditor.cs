using System.Linq;
using UnityEditor;
using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.Audio.Editor
{
    [CustomNodeEditor(typeof(StartAudioNode))]
    public class StartAudioNodeEditor : NodeEditor
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
                    case "audioSourceLocation":
                    case "audioClip":
                    case "audioMixer":
                    case "mute":
                    case "isPublic":
                    case "bypassEffects":
                    case "bypassListenerEffects":
                    case "bypassReverbZones":
                    case "loop":
                        EditorGUIUtility.labelWidth = 150;
                        NodeEditorGUILayout.PropertyField(iterator);
                        break;
                    case "publicId":
                        var isPublic = serializedObject.FindProperty("isPublic");
                        if (isPublic is {boolValue: true})
                        {
                            EditorGUIUtility.labelWidth = 150;
                            NodeEditorGUILayout.PropertyField(iterator);
                        }
                        else
                        {
                            //Clear previous value
                            iterator.stringValue = string.Empty;
                        }

                        break;
                    case "targetVolume":
                    case "fadeDuration":
                    case "initialFadeDelay":
                        var audioFade = serializedObject.FindProperty("audioFade");
                        if (audioFade is {boolValue: true})
                        {
                            EditorGUIUtility.labelWidth = 110;
                            NodeEditorGUILayout.PropertyField(iterator);
                        }
                        else
                        {
                            //Clear previous value
                            iterator.floatValue = 0f;
                        }

                        break;
                    default:
                        EditorGUIUtility.labelWidth = 110;
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
    }
}