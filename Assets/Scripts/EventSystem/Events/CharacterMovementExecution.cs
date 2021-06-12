using System.Collections;
using EventSystem.Events.interfaces;
using EventSystem.Events.Models;
using EventSystem.Models;
using UnityEngine;
using UnityEngine.AI;

namespace EventSystem.Events
{
    public class CharacterMovementExecution : IEventExecution
    {
        private CharacterMovement _characterMovement;
        
        public IEnumerator Execute(GameEvent gameEvent)
        {
            //Copy object before delay to prevent a null on IsFinished check
            _characterMovement = gameEvent.characterMovement;
            
            //Initial delay
            yield return (gameEvent.initialDelayTime > 0 ? new WaitForSeconds(gameEvent.initialDelayTime) : null);
            
            //Create navMeshAgent
            var targetNavMeshAgent = _characterMovement.target.GetComponent<NavMeshAgent>();
            if (targetNavMeshAgent == null)
            {
                Debug.LogError($"{nameof(CharacterMovementExecution)}: Missing component {nameof(NavMeshAgent)}");
                yield return null;
            }
            
            //Teleport if starting position given
            if (_characterMovement.startingPosition != null)
            {
                targetNavMeshAgent.Warp(_characterMovement.startingPosition.transform.position);
            }

            //Move to position
            targetNavMeshAgent.SetDestination(_characterMovement.targetPosition.transform.position);
        }

        //Check if objects position is within range of the target position
        public bool IsFinished()
        {
            return (Vector3.Distance(_characterMovement.target.transform.position, _characterMovement.targetPosition.transform.position) <= _characterMovement.distanceThreshold);
        }
    }
}