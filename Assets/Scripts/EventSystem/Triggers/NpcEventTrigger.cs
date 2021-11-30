using System.Collections;
using Characters;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;

namespace EventSystem.Triggers
{
    /// <summary>
    /// Attach to an NPC character to provide default and triggered behaviour for the NPC gameObject
    /// The player must be within range and press the confirm button
    /// </summary>
    [RequireComponent(typeof(CharacterManager))]
    public class NpcEventTrigger : MonoBehaviour
    {
        //Used to control focus
        private CharacterManager _characterManager;

        [Tooltip("Event Sequence to be triggered")]
        public EventSequenceGraph triggerEventSequence;

        private EventSequenceParser _triggerEventSequenceParser;
        private bool _eventRunning;

        private void Start()
        {
            _characterManager = GetComponent<CharacterManager>();
        }

        /**
         * We have 3 problems here:
         * 1: FIXED: This is getting called repeatedly because we currently dont require any input to trigger
         * 
         * 2: FIXED: Once the trigger event sequence has been called once it can't be called again because the nodes are
         * still marked as finished. We need a method to reset the nodes
         *
         * 3: FIXED: The default event sequence is still running in the background bc coroutines. We need a way to PAUSE, then
         * start the coroutine. We do not want to restart the sequence from the beginning.
         */
        public IEnumerator BeginTriggerEvent(GameObject triggerObject, CharacterManager triggerCharacterManager)
        {
            if (triggerEventSequence == null || _eventRunning)
                yield break;

            _eventRunning = true;

            //Pause events of main sequence
            _characterManager.PauseEventSequence();

            //Add trigger event timeline parser
            if (_triggerEventSequenceParser == null)
            {
                _triggerEventSequenceParser = gameObject.AddComponent<EventSequenceParser>();
            }

            //Start trigger event sequence
            StartCoroutine(_triggerEventSequenceParser.StartEventSequence(triggerEventSequence));

            _characterManager.SetFocus(triggerObject);
            triggerCharacterManager.SetFocus(this.gameObject);

            yield return new WaitUntil(_triggerEventSequenceParser.IsEventSequenceFinished);

            //Remove focus Events
            triggerCharacterManager.RemoveFocus();
            _characterManager.RemoveFocus();

            //Resume events
            _characterManager.ResumeEventSequence();
            _eventRunning = false;
        }
    }
}