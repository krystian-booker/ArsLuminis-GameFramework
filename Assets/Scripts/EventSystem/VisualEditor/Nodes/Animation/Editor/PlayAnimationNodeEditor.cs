using System;
using System.Linq;
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
            onUpdateNode += UpdateSelectedAnimationTrigger;
        }

        /// Node Width
        public override int GetWidth()
        {
            return 350;
        }

        private int _selectedIndex;
        private string[] _animationTriggers = Array.Empty<string>();
        private GameObject _currentAnimationTarget;

        public override void OnBodyGUI()
        {
            serializedObject.Update();
            string[] excludes = {"m_Script", "graph", "position", "ports"};

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            GetAnimationTriggers();
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
                        _selectedIndex = EditorGUILayout.Popup("Animation Trigger", _selectedIndex, _animationTriggers);
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

        /// <summary>
        /// Select animation trigger automatically
        /// </summary>
        /// <param name="node"></param>
        private void UpdateSelectedAnimationTrigger(Node node)
        {
            var playAnimationNode = node as PlayAnimationNode;
            if (playAnimationNode == null || _animationTriggers == null || !_animationTriggers.Any())
                return;

            playAnimationNode.animationTrigger = _animationTriggers[_selectedIndex];
        }

        /// <summary>
        /// This will automatically get the link of animations for the provided gameObject
        /// The animations must first be defined in the Models/Animations folder
        /// </summary>
        private void GetAnimationTriggers()
        {
            var animationTargetProperty = serializedObject.FindProperty("animationTarget");
            var animationTarget = animationTargetProperty.objectReferenceValue as GameObject;

            if (animationTarget == null || _animationTriggers?.Length > 0 && _currentAnimationTarget == animationTarget)
                return;

            _currentAnimationTarget = animationTarget;

            var animator = animationTarget.GetComponent<Animator>();
            Assert.IsNotNull(animator, $"{nameof(PlayAnimationNode)}: Missing component ${nameof(Animator)} on {animationTarget.name}");

            var animatorController = animator.runtimeAnimatorController as AnimatorController;
            Assert.IsNotNull(animatorController, $"{nameof(PlayAnimationNode)}: Missing ${nameof(AnimatorController)} on {animationTarget.name}");

            _animationTriggers = animatorController.parameters.Select(x => x.name).ToArray();
        }
    }
}