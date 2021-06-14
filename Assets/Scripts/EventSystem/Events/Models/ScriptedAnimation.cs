using System;
using Animations;
using EditorTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem.Events.Models
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    public class ScriptedAnimation
    {
        [Tooltip("GameObject you want the animation to run on.")]
        [OnValueChanged(nameof(GETAnimationTriggers))]
        [InlineButton(nameof(GETAnimationTriggers), "Refresh")]
        public GameObject animationTarget;

        [ValueDropdown("_animationTriggers")]
        [DisableIf("@this._animationTriggers == null || this._animationTriggers.Length == 0")]
        public string animationTrigger;
        
#if UNITY_EDITOR
        private string[] _animationTriggers;
        
        //I'm either brilliant or an idiot
        private void GETAnimationTriggers()
        {
            try
            {
                if (animationTarget == null) return;
                var animator = animationTarget.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError(
                        $"{nameof(ScriptedAnimation)}: Missing component ${nameof(Animator)} on {animationTarget.name}");
                }

                var runtimeAnimatorController = animator.runtimeAnimatorController;
                if (runtimeAnimatorController == null)
                {
                    Debug.LogError(
                        $"{nameof(ScriptedAnimation)}: Missing ${nameof(RuntimeAnimatorController)} on {animationTarget.name}");
                }

                var animatorType = Tools.GetEnumType($"Animations.{runtimeAnimatorController.name}");
                if (animatorType == null)
                {
                    Debug.LogError($"{nameof(ScriptedAnimation)}: Unable to find matching enum of " +
                                   $"type {runtimeAnimatorController.name}. \n Did you forget to create the enum in " +
                                   $"'Scripts/Animations' with a matching name and properties to the animator controller ");
                }

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