using System;
using System.Collections;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Actions;
using UnityEngine;
using UnityEngine.AI;
using XNode;

namespace EventSystem.Events
{
    public class CharacterMovementExecution : IEventExecution
    {
        private CharacterMovementNode _characterMovementNode;
        private NavMeshAgent _targetNavMeshAgent;

        public IEnumerator Execute(Node node)
        {
            _characterMovementNode = node as CharacterMovementNode;
            if (_characterMovementNode != null)
            {
                //Get navMeshAgent
                _targetNavMeshAgent = _characterMovementNode.target.GetComponent<NavMeshAgent>();
                if (_targetNavMeshAgent == null)
                {
                    Debug.LogError($"{nameof(CharacterMovementExecution)}: Missing component {nameof(NavMeshAgent)}");
                }

                //Set navmeshagent properties
                _targetNavMeshAgent.speed = _characterMovementNode.speed;
                _targetNavMeshAgent.updateRotation = !_characterMovementNode.disableRotation;

                //Teleport if starting position given
                if (_characterMovementNode.startingPosition != null)
                {
                    _targetNavMeshAgent.Warp(_characterMovementNode.startingPosition.transform.position);
                }

                //Move to position
                _targetNavMeshAgent.SetDestination(_characterMovementNode.targetPosition.transform.position);
                yield return null;
            }
            else
            {
                Debug.LogException(new Exception($"{nameof(CharacterMovementExecution)}: Invalid setup on {nameof(CharacterMovementNode)}."));
            }
        }

        //Check if objects position is within range of the target position
        public bool IsFinished()
        {
            return _targetNavMeshAgent != null && _targetNavMeshAgent.hasPath && _targetNavMeshAgent.remainingDistance <=
                _targetNavMeshAgent.stoppingDistance + _characterMovementNode.distanceThreshold;
        }
    }
}