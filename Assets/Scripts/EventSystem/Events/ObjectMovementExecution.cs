using System.Collections;
using EditorTools;
using EventSystem.Events.interfaces;
using EventSystem.Events.Models;
using UnityEngine;
using UnityEngine.AI;

namespace EventSystem.Events
{
    public class ObjectMovementExecution : IEventExecution
    {
        private ObjectMovement _objectMovement;
        private NavMeshAgent _targetNavMeshAgent;
        
        public IEnumerator Execute(GameEvent gameEvent)
        {
            //Copy object before delay to prevent a null on IsFinished check
            _objectMovement = gameEvent.objectMovement;
            
            //Initial delay
            yield return (gameEvent.initialDelayTime > 0 ? new WaitForSeconds(gameEvent.initialDelayTime) : null);
            
            //Create navMeshAgent
            _targetNavMeshAgent = _objectMovement.target.GetComponent<NavMeshAgent>();
            
            //Expected to be null
            if (_targetNavMeshAgent == null)
            {
                _targetNavMeshAgent = _objectMovement.target.AddComponent<NavMeshAgent>();
            }
            
            //Set navmeshagent properties
            _targetNavMeshAgent.speed = _objectMovement.speed;
            _targetNavMeshAgent.updateRotation = !_objectMovement.disableRotation;
            _targetNavMeshAgent.radius = _objectMovement.navMeshRadius;
                
            //Teleport if starting position given
            if (_objectMovement.startingPosition != null)
            {
                _targetNavMeshAgent.Warp(_objectMovement.startingPosition.transform.position);
            }

            //Move to position
            _targetNavMeshAgent.SetDestination(_objectMovement.targetPosition.transform.position);
        }

        //Check if objects position is within range of the target position of x,y
        public bool IsFinished()
        {
            if (_targetNavMeshAgent == null || !(_targetNavMeshAgent.remainingDistance <=
                                                 _targetNavMeshAgent.stoppingDistance +
                                                 _objectMovement.distanceThreshold)) return false;
            Tools.DestroyComponent(_targetNavMeshAgent);
            return true;

        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}