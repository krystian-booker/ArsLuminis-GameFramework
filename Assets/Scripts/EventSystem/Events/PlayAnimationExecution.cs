using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Animation;
using Tools;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class AnimationExecution : IEventExecution
    {
        private PlayAnimationNode _animationNode;
        private Animator _animator;
        private AnimatorLink _animatorLink;

        public void Execute(Node node)
        {
            _animationNode = node as PlayAnimationNode;
            Assert.IsNotNull(_animationNode, $"{nameof(AnimationExecution)}: Invalid setup on {nameof(PlayAnimationNode)}.");

            //Get animator
            _animator = _animationNode.animationTarget.GetComponent<Animator>();
            _animatorLink = _animationNode.animationTarget.GetComponent<AnimatorLink>();

            //Asserts
            Assert.IsNotNull(_animator, $"{nameof(AnimationExecution)}: Animator required for playing animations on ${_animationNode.animationTarget.name}");
            Assert.IsNotNull(_animatorLink,$"{nameof(AnimationExecution)}: AnimatorLink required for playing animations on ${_animationNode.animationTarget.name}");

            //Clear previous states
            _animatorLink.ResetAnimationState();

            //Start animation
            _animator.SetTrigger(_animationNode.animationTrigger);
        }

        public bool IsFinished()
        {
            return !_animationNode.waitForAnimationEvent || _animatorLink.IsAnimationComplete();
        }
    }
}