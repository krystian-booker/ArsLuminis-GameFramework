using System;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Actions;
using UnityEngine.AI;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class CharacterMovementExecution : IPauseEventExecution
    {
        private CharacterMovementNode _characterMovementNode;
        private NavMeshAgent _targetNavMeshAgent;

        public void Execute(Node node)
        {
            _characterMovementNode = node as CharacterMovementNode;
            Assert.IsNotNull(_characterMovementNode,
                $"{nameof(CharacterMovementExecution)}: Invalid setup on {nameof(CharacterMovementNode)}.");

            //Get navMeshAgent
            _targetNavMeshAgent = _characterMovementNode.target.GetComponent<NavMeshAgent>();
            Assert.IsNotNull(_targetNavMeshAgent,
                $"{nameof(CharacterMovementExecution)}: Missing component {nameof(NavMeshAgent)}");

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
        }

        //Check if objects position is within range of the target position
        public bool IsFinished()
        {
            return _targetNavMeshAgent != null && _targetNavMeshAgent.hasPath &&
                   _targetNavMeshAgent.remainingDistance <=
                   _targetNavMeshAgent.stoppingDistance + _characterMovementNode.distanceThreshold;
        }

        public void PauseExecution()
        {
            _targetNavMeshAgent.isStopped = true;
        }

        public void ResumeExecution()
        {
            _targetNavMeshAgent.isStopped = false;
        }
    }
}