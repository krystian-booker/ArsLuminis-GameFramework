using UnityEngine;

namespace Tools
{
    public class AnimatorLink : MonoBehaviour
    {
        private bool _animationFinished;

        //Unity animation event call
        public void MarkAnimationComplete()
        {
            _animationFinished = true;
        }

        public void ResetAnimationState()
        {
            _animationFinished = false;
        }

        public bool IsAnimationComplete()
        {
            return _animationFinished;
        }
    }
}