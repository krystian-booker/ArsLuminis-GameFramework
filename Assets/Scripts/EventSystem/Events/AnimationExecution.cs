using System;
using System.Collections;
using Animations;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Actions;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class AnimationExecution : IEventExecution
    {
        private AnimationNode _animationNode;
        private Animator _animator;
        private AnimatorLink _animatorLink;

        public void Execute(Node node)
        {
            _animationNode = node as AnimationNode;
            Assert.IsNotNull(_animationNode,
                $"{nameof(AnimationExecution)}: Invalid setup on {nameof(AnimationNode)}.");

            //Get animator
            _animator = _animationNode.animationTarget.GetComponent<Animator>();
            _animatorLink = _animationNode.animationTarget.GetComponent<AnimatorLink>();

            //Asserts
            Assert.IsNotNull(_animator,
                $"{nameof(AnimationExecution)}: Animator required for playing animations on ${_animationNode.animationTarget.name}");
            Assert.IsNotNull(_animatorLink,
                $"{nameof(AnimationExecution)}: AnimatorLink required for playing animations on ${_animationNode.animationTarget.name}");

            //Clear previous states
            _animatorLink.ResetAnimationState();

            //Start animation
            _animator.SetTrigger(_animationNode.animationTrigger);
        }

        public bool IsFinished()
        {
            return !_animationNode.continueOnAnimationEvent || _animatorLink.IsAnimationComplete();
        }

        //Unused
        public void OnDropObjects(UnityEngine.Object[] objects)
        {
        }
    }
}