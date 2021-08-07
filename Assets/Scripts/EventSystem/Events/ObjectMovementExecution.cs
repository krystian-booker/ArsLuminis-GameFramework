using System;
using EditorTools;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class ObjectMovementExecution : IPauseEventExecution
    {
        private ObjectMovementNode _objectMovementNode;
        private NavMeshAgent _targetNavMeshAgent;

        public void Execute(Node node)
        {
            _objectMovementNode = node as ObjectMovementNode;
            Assert.IsNotNull(_objectMovementNode,
                $"{nameof(ObjectMovementExecution)}: Invalid setup on {nameof(ObjectMovementNode)}.");

            //Create navMeshAgent
            _targetNavMeshAgent = _objectMovementNode.target.GetComponent<NavMeshAgent>();

            //Expected to be null
            if (_targetNavMeshAgent == null)
            {
                _targetNavMeshAgent = _objectMovementNode.target.AddComponent<NavMeshAgent>();
            }

            //Set navmeshagent properties
            _targetNavMeshAgent.speed = _objectMovementNode.speed;
            _targetNavMeshAgent.updateRotation = !_objectMovementNode.disableRotation;
            _targetNavMeshAgent.radius = _objectMovementNode.navMeshRadius;

            //Teleport if starting position given
            if (_objectMovementNode.startingPosition != null)
            {
                _targetNavMeshAgent.Warp(_objectMovementNode.startingPosition.transform.position);
            }

            //Move to position
            _targetNavMeshAgent.SetDestination(_objectMovementNode.targetPosition.transform.position);
        }

        public bool IsFinished()
        {
            //Check if objects position is within range of the target position of x,y
            if (_targetNavMeshAgent == null || !_targetNavMeshAgent.hasPath ||
                !(_targetNavMeshAgent.remainingDistance <=
                  _targetNavMeshAgent.stoppingDistance +
                  _objectMovementNode.distanceThreshold)) return false;

            //Remove Navmesh
            Tools.DestroyComponent(_targetNavMeshAgent);
            return true;
        }

        public void PauseExecution()
        {
            _targetNavMeshAgent.isStopped = true;
        }

        public void ResumeExecution()
        {
            _targetNavMeshAgent.isStopped = false;
            _targetNavMeshAgent.SetDestination(_objectMovementNode.targetPosition.transform.position);
        }
    }
}