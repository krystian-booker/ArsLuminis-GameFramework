using System.Collections;
using EventSystem.Events.interfaces;
using EventSystem.Events.Models;
using EventSystem.Models;
using UnityEngine;
using UnityEngine.AI;

namespace EventSystem.Events
{
    public class ObjectMovementExecution : IEventExecution
    {
        private ObjectMovement _objectMovement;
        
        public IEnumerator Execute(GameEvent gameEvent)
        {
            //Copy object before delay to prevent a null on IsFinished check
            _objectMovement = gameEvent.objectMovement;
            
            //Initial delay
            yield return (gameEvent.initialDelayTime > 0 ? new WaitForSeconds(gameEvent.initialDelayTime) : null);
            
            //Create navMeshAgent
            var targetNavMeshAgent = _objectMovement.target.GetComponent<NavMeshAgent>();
            
            //Expected to be null
            if (targetNavMeshAgent == null)
            {
                targetNavMeshAgent = _objectMovement.target.AddComponent<NavMeshAgent>();
            }
            
            //Set properties
            targetNavMeshAgent.speed = _objectMovement.speed;
            
            //Teleport if starting position given
            if (_objectMovement.startingPosition != null)
            {
                targetNavMeshAgent.Warp(_objectMovement.startingPosition.transform.position);
            }

            //Move to position
            targetNavMeshAgent.SetDestination(_objectMovement.targetPosition.transform.position);
        }

        //Check if objects position is within range of the target position
        public bool IsFinished()
        {
            return (Vector3.Distance(_objectMovement.target.transform.position, _objectMovement.targetPosition.transform.position) <= _objectMovement.distanceThreshold);
        }
    }
}