using Assets.Scripts.Managers;
using Assets.Scripts.Nodes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Nodes.Flow
{
    [NodeTint(100, 0, 100)]
    public class WaitForInput : ExecutableNode
    {
        [Tooltip("The name of the button that should be pressed to continue.")]
        [SerializeField] private string TriggerAction = "Interact";

        private bool _isFinished;

        public override void Execute()
        {
            _isFinished = false;

            // Fetch the InputAction by name from the PlayerInput component
            InputAction interactAction = GameManager.Instance.PlayerInput.actions.FindAction(TriggerAction);
            Assert.IsNotNull(interactAction, string.Format("PlayerInput is missing the action {0}", TriggerAction));

            // Register the performed callback
            interactAction.performed += OnInteractPerformed;
            interactAction.Enable();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _isFinished = true;
            context.action.performed -= OnInteractPerformed;
        }

        public override bool IsFinished()
        {
            return _isFinished;
        }
    }
}
