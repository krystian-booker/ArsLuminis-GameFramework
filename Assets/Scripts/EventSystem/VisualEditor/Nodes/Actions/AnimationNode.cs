using System;
using EditorTools;
using EventSystem.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#577590")]
    public class AnimationNode : BaseNodeExtended
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Documentation purposes only")] [TextArea]
        public string description;

        [LabelWidth(250),
         Tooltip(
             "The event sequence will only continue when animation event runs. If disabled, the event sequence will continue instantly.")]
        public bool continueOnAnimationEvent = true;

        [LabelWidth(100), Tooltip("GameObject you want the animation to run on.")]
        [OnValueChanged(nameof(GETAnimationTriggers))]
        public GameObject animationTarget;

        //TODO: Remove 'ValueDropdown', create custom UI 
        [LabelWidth(100), ValueDropdown("_animationTriggers")]
        [DisableIf("@this._animationTriggers == null || this._animationTriggers.Length == 0")]
        public string animationTrigger;

#if UNITY_EDITOR

        private string[] _animationTriggers; //IS USED BY ODIN

        /// <summary>
        /// This will automatically get the link of animations for the provided gameObject
        /// The animations must first be defined in the Models/Animations folder
        /// </summary>
        //TODO: Remove 'Button', create custom UI 
        [Button("Refresh")]
        [OnInspectorGUI("GETAnimationTriggers")]
        private void GETAnimationTriggers()
        {
            try
            {
                if (animationTarget == null) return;
                var animator = animationTarget.GetComponent<Animator>();
                Assert.IsNotNull(animator,
                    $"{nameof(AnimationNode)}: Missing component ${nameof(Animator)} on {animationTarget.name}");

                var runtimeAnimatorController = animator.runtimeAnimatorController;
                Assert.IsNotNull(runtimeAnimatorController,
                    $"{nameof(AnimationNode)}: Missing ${nameof(RuntimeAnimatorController)} on {animationTarget.name}");

                var animatorType = Tools.GetEnumType($"Models.Animations.{runtimeAnimatorController.name}");
                Assert.IsNotNull(animatorType, $"{nameof(AnimationNode)}: Unable to find matching enum of " +
                                               $"type {runtimeAnimatorController.name}. \n Did you forget to create the enum in " +
                                               $"'Scripts/Animations' with a matching name and properties to the animator controller ");

                var enumValues = animatorType?.GetEnumNames();
                _animationTriggers = enumValues;
            }
            catch (Exception)
            {
                // ignored
            }
        }
#endif
    }
}