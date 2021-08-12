using System;
using System.Linq;
using Tools;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;
using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.Animation.Editor
{
    [CustomNodeEditor(typeof(PlayAnimationNode))]
    public class PlayAnimationNodeEditor : NodeEditor
    {
        public PlayAnimationNodeEditor()
        {
            //Builtin action on xNode
            onUpdateNode += GetAnimationTriggers;
            onUpdateNode += UpdateSelectedAnimationTrigger;
        }

        /// <summary>
        /// Node Width
        /// </summary>
        /// <returns></returns>
        public override int GetWidth()
        {
            return 350;
        }

        public int selectedIndex;
        public string[] animationTriggers = Array.Empty<string>();

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
                switch (iterator.name)
                {
                    case "continueOnAnimationEvent":
                        EditorGUIUtility.labelWidth = 300;
                        NodeEditorGUILayout.PropertyField(iterator);
                        break;
                    case "animationTrigger":
                        selectedIndex = EditorGUILayout.Popup("Animation Trigger", selectedIndex, animationTriggers);
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

        private void UpdateSelectedAnimationTrigger(Node node)
        {
            var playAnimationNode = node as PlayAnimationNode;
            if (playAnimationNode == null || animationTriggers == null || !animationTriggers.Any())
                return;
            
            playAnimationNode.animationTrigger = animationTriggers[selectedIndex];
        }

        /// <summary>
        /// This will automatically get the link of animations for the provided gameObject
        /// The animations must first be defined in the Models/Animations folder
        /// </summary>
        private void GetAnimationTriggers(Node node)
        {
            var playAnimationNode = node as PlayAnimationNode;
            if (playAnimationNode == null || playAnimationNode.animationTarget == null ||
                animationTriggers?.Length > 0)
                return;

            var animator = playAnimationNode.animationTarget.GetComponent<Animator>();
            Assert.IsNotNull(animator,
                $"{nameof(PlayAnimationNode)}: Missing component ${nameof(Animator)} on {playAnimationNode.animationTarget.name}");

            var runtimeAnimatorController = animator.runtimeAnimatorController;
            Assert.IsNotNull(runtimeAnimatorController,
                $"{nameof(PlayAnimationNode)}: Missing ${nameof(RuntimeAnimatorController)} on {playAnimationNode.animationTarget.name}");

            var animatorController = runtimeAnimatorController as AnimatorController;
            Assert.IsNotNull(animatorController,
                $"{nameof(PlayAnimationNode)}: Missing ${nameof(AnimatorController)} on {playAnimationNode.animationTarget.name}");

            animationTriggers = animatorController.parameters.Select(x => x.name).ToArray();
        }
    }
}