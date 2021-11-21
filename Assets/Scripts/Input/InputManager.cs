using Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SystemInput
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        public PlayerInput playerInput;

        private Vector3 _rawInputMovement;

        private void FixedUpdate()
        {
            if (_rawInputMovement != Vector3.zero)
            {
                Systems.GameManager.activeCharacterManager.Move(_rawInputMovement);
            }
        }

        public void ChangeActionMap(ActionMap actionMap)
        {
            playerInput.SwitchCurrentActionMap(actionMap.ToString());
        }

        #region Menu Actions

        #endregion

        #region Dialog Actions

        public void OnDialogConfirm(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                Systems.DialogManager.ContinueClicked();
            }
        }

        #endregion

        #region Movement Actions

        public InputAction.CallbackContext onConfirmValue;
        public InputAction.CallbackContext onCancelValue;
        public InputAction.CallbackContext onMenuValue;
        public InputAction.CallbackContext onActionValue;

        public void OnMovement(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();

            //camera forward and right vectors:
            var cameraTransform = Systems.CinemachineBrain.transform;
            var cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;

            var cameraRight = cameraTransform.right;
            cameraRight.y = 0f;

            //project forward and right vectors on the horizontal plane (y = 0)
            cameraForward.Normalize();
            cameraRight.Normalize();

            //this is the direction in the world space we want to move:
            _rawInputMovement = cameraForward * inputMovement.y + cameraRight * inputMovement.x;
        }


        public void OnConfirm(InputAction.CallbackContext value)
        {
            onConfirmValue = value;
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            onActionValue = value;
        }

        public void OnCancel(InputAction.CallbackContext value)
        {
            onCancelValue = value;
        }

        public void OnMenu(InputAction.CallbackContext value)
        {
            onMenuValue = value;
        }

        #endregion Movement

        #region Global Actions

        public void OnPause(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                //Do action
                Debug.Log("Pause/Unpause");
            }
        }

        #endregion
    }
}