using Characters;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Locomotion;
using Tools;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class ObjectMovementExecution : IPauseEventExecution
    {
        private ObjectMovementNode _objectMovementNode;
        private CharacterManager _targetCharacterManager;

        public void Execute(Node node)
        {
            _objectMovementNode = node as ObjectMovementNode;
            Assert.IsNotNull(_objectMovementNode, $"{nameof(ObjectMovementExecution)}: Invalid setup on {nameof(ObjectMovementNode)}.");

            //Create navMeshAgent
            _targetCharacterManager = _objectMovementNode.target.GetComponent<CharacterManager>();
            Assert.IsNotNull($"{nameof(ObjectMovementExecution)}: CharacterManager required.");

            //Set properties

            //TODO: Add AngularSpeed, Velocity, Acceleration
            _targetCharacterManager.SetSpeed(_objectMovementNode.speed);
            _targetCharacterManager.UpdateRotation(!_objectMovementNode.disableRotation);
            _targetCharacterManager.SetRadius(_objectMovementNode.navMeshRadius);

            //Teleport if starting position given
            if (_objectMovementNode.startingPosition != null)
            {
                _targetCharacterManager.Warp(_objectMovementNode.startingPosition.transform.position);
            }

            //Move to position
            _targetCharacterManager.SetDestination(_objectMovementNode.targetPosition.transform.position);
        }

        public bool IsFinished()
        {
            //Check if objects position is within range of the target position of x,y
            if (_targetCharacterManager == null || !_targetCharacterManager.HasPath() || !(_targetCharacterManager.GetRemainingDistance() <=
                                                                                           _targetCharacterManager.GetStoppingDistance() +
                                                                                           _objectMovementNode.distanceThreshold))
                return false;

            //Remove Navmesh
            Utilities.DestroyComponent(_targetCharacterManager);
            return true;
        }

        public void PauseExecution()
        {
            _targetCharacterManager.IsStopped(true);
        }

        public void ResumeExecution()
        {
            _targetCharacterManager.IsStopped(false);
            _targetCharacterManager.SetDestination(_objectMovementNode.targetPosition.transform.position);
        }
    }
}