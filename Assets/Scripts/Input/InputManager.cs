using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        private Vector3 _rawInputMovement;

        private void FixedUpdate()
        {
            if (_rawInputMovement == Vector3.zero) return;
            
            GameManager.Instance.ActiveCharacterNavMeshAgent.Move(_rawInputMovement * 0.5f);
            GameManager.Instance.activePlayer.transform.rotation = Quaternion.LookRotation(_rawInputMovement);
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();
         
            //camera forward and right vectors:
            var cameraTransform = GameManager.Instance.cinemachineBrain.transform;
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
            if (value.started)
            {
                //Do action
                Debug.Log("Confirm button pressed");
            }
        }
    }
}
