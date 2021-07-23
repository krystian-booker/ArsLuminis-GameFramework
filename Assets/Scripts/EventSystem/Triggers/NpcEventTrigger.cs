using System.Collections;
using Characters;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;

namespace EventSystem.Triggers
{
    [RequireComponent(typeof(CharacterManager))]
    public class NpcEventTrigger : MonoBehaviour
    {
        private CharacterManager _characterManager;
        
        public EventSequenceSceneGraph triggerEventSequence;
        private EventTimelineParser _triggerEventTimelineParser;
        
        [Tooltip("Should the trigger event sequence be replayable? If unchecked event sequence can only run once.")]
        public bool resetTriggerSequence = true;

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
            if (triggerEventSequence == null)
                yield return null;
            
            //Pause events of main sequence
            _characterManager.PauseEventSequence();

            //Add trigger event timeline parser
            if (_triggerEventTimelineParser == null)
            {
                _triggerEventTimelineParser = gameObject.AddComponent<EventTimelineParser>();
            }
            
            //Start trigger event sequence
            StartCoroutine(_triggerEventTimelineParser.StartEventSequence(triggerEventSequence));
            
            //TODO: thought: Should these focus events be a setting on the eventSequence?
            _characterManager.SetFocus(triggerObject);
            triggerCharacterManager.SetFocus(this.gameObject);
            
            yield return new WaitUntil(_triggerEventTimelineParser.IsEventSequenceFinished);
            
            //Remove focus Events
            triggerCharacterManager.LoseFocus();
            _characterManager.LoseFocus();
            
            //Reset trigger event sequence 
            if (resetTriggerSequence)
            {
                GameManager.Instance.eventSystemManager.ResetEventSequenceSceneGraph(triggerEventSequence);
            }
            
            //Resume events
            _characterManager.ResumeEventSequence();
        }
    }
}