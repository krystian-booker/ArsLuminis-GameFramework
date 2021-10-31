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
                Systems.GameManager.activeCharacterNavMeshAgent.Move(_rawInputMovement * 0.5f);
                Systems.GameManager.activePlayer.transform.rotation = Quaternion.LookRotation(_rawInputMovement);
            }
        }

        public void ChangeActionMap(ActionMap actionMap)
        {
            playerInput.SwitchCurrentActionMap(actionMap.ToString());
        }

        #region Menu

        #endregion

        #region Dialog

        public void OnDialogConfirm(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                Systems.DialogManager.ContinueClicked();
            }
        }

        #endregion

        #region Movement

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

        #region Global

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