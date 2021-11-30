using Characters;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Locomotion;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class CharacterMovementExecution : IPauseEventExecution
    {
        private CharacterMovementNode _characterMovementNode;
        private CharacterManager _targetCharacterManager;

        public void Execute(Node node)
        {
            _characterMovementNode = node as CharacterMovementNode;
            Assert.IsNotNull(_characterMovementNode, $"{nameof(CharacterMovementExecution)}: Invalid setup on {nameof(CharacterMovementNode)}.");

            //Get navMeshAgent
            _targetCharacterManager = _characterMovementNode.target.GetComponent<CharacterManager>();
            Assert.IsNotNull(_targetCharacterManager, $"{nameof(CharacterMovementExecution)}: Missing component {nameof(CharacterManager)}");

            //Set properties
            _targetCharacterManager.SetSpeed(_characterMovementNode.speed);
            _targetCharacterManager.UpdateRotation(!_characterMovementNode.disableRotation);

            //Teleport if starting position given
            if (_characterMovementNode.startingPosition != null)
            {
                _targetCharacterManager.Warp(_characterMovementNode.startingPosition.transform.position);
            }

            //Move to position
            _targetCharacterManager.SetDestination(_characterMovementNode.targetPosition.transform.position);
        }

        //Check if objects position is within range of the target position
        public bool IsFinished()
        {
            return _targetCharacterManager != null && _targetCharacterManager.HasPath() &&
                   _targetCharacterManager.GetRemainingDistance() <= _targetCharacterManager.GetStoppingDistance() + _characterMovementNode.distanceThreshold;
        }

        public void PauseExecution()
        {
            _targetCharacterManager.IsStopped(true);
        }

        public void ResumeExecution()
        {
            _targetCharacterManager.IsStopped(false);
        }
    }
}