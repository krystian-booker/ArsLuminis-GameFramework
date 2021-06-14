using System.Collections;
using EventSystem.Events.interfaces;
using EventSystem.Events.Models;
using UnityEngine;

namespace EventSystem.Events
{
    public class AnimationExecution : IEventExecution
    {
        private ScriptedAnimation _scriptedAnimation;
        private Animator _animator;
        
        public IEnumerator Execute(GameEvent gameEvent)
        {
            //Copy object before delay to prevent a null on IsFinished check
            _scriptedAnimation = gameEvent.scriptedAnimation;
                        
            //Initial delay
            yield return (gameEvent.initialDelayTime > 0 ? new WaitForSeconds(gameEvent.initialDelayTime) : null);

            //Get animator
            _animator = _scriptedAnimation.animationTarget.GetComponent<Animator>();
            
            //Start animation
            //TODO: Implement booleans, maybe floats and int? 
            _animator.SetTrigger(_scriptedAnimation.animationTrigger);
        }

        public bool IsFinished()
        {
            return (_animator.GetCurrentAnimatorStateInfo(0).IsName("TrainMovesToPlatform"));
            //Animation event :o
            //throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}