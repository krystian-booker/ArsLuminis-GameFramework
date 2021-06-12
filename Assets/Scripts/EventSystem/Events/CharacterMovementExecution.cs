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
        private NavMeshAgent _targetNavMeshAgent;
        
        public IEnumerator Execute(GameEvent gameEvent)
        {
            //Copy object before delay to prevent a null on IsFinished check
            _characterMovement = gameEvent.characterMovement;
            
            //Initial delay
            yield return (gameEvent.initialDelayTime > 0 ? new WaitForSeconds(gameEvent.initialDelayTime) : null);
            
            //Create navMeshAgent
            
            _targetNavMeshAgent = _characterMovement.target.GetComponent<NavMeshAgent>();
            if (_targetNavMeshAgent == null)
            {
                Debug.LogError($"{nameof(CharacterMovementExecution)}: Missing component {nameof(NavMeshAgent)}");
                yield return null;
            }

            //Set navmeshagent properties
            _targetNavMeshAgent.updateRotation = !_characterMovement.disableRotation;
            
            //Teleport if starting position given
            if (_characterMovement.startingPosition != null)
            {
                _targetNavMeshAgent.Warp(_characterMovement.startingPosition.transform.position);
            }

            //Move to position
            _targetNavMeshAgent.SetDestination(_characterMovement.targetPosition.transform.position);
        }

        //Check if objects position is within range of the target position
        public bool IsFinished()
        {
            return (_targetNavMeshAgent != null && _targetNavMeshAgent.remainingDistance <=
                _targetNavMeshAgent.stoppingDistance + _characterMovement.distanceThreshold);
        }
    }
}