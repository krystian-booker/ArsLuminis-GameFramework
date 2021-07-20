using EventSystem.Triggers;
using UnityEngine;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            //Only want events triggered on active player
            if (GameManager.Instance.activePlayer.GetInstanceID() != gameObject.GetInstanceID()) return;
            
            //TODO: Remove both nameToLayer and CompareTag, replace of enum for both
            if (other.gameObject.layer != LayerMask.NameToLayer("Character") ||
                !other.gameObject.CompareTag("NPC")) return;
            
            //InputManager wait for a confirm
            if (!GameManager.Instance.inputManager.onConfirmValue.started) return;

            //How can we avoid this GetComponent?
            var npcEventTrigger = other.gameObject.GetComponent<NpcEventTrigger>();
            npcEventTrigger.BeginTriggerEvent();
        }
    }
}