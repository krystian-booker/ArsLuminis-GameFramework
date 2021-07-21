using System.Collections;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EventSystem.Triggers
{
    public class NpcEventTrigger : MonoBehaviour
    {
        public EventSequenceSceneGraph defaultEventSequence;
        private EventTimelineParser _defaultEventTimelineParser;

        public EventSequenceSceneGraph triggerEventSequence;
        private EventTimelineParser _triggerEventTimelineParser;
        
        [Tooltip("Should the trigger event sequence be replayable? If unchecked event sequence can only run once.")]
        public bool resetTriggerSequence = true;

        [Tooltip("If set to true this gameObject will turn towards the triggering game object (player)")]
        public bool triggerFocusOnPlayer = true;
        
        private GameObject _triggerGameObject;
        private bool _focusActive;
        
        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            Assert.IsNotNull(defaultEventSequence);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _defaultEventTimelineParser = gameObject.AddComponent<EventTimelineParser>();
            StartCoroutine(_defaultEventTimelineParser.StartEventSequence(defaultEventSequence));
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (_focusActive && _triggerGameObject != null)
            {
                FocusOnTrigger();
            }
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
        public IEnumerator BeginTriggerEvent(GameObject triggerGO)
        {
            if (triggerEventSequence == null)
                yield return null;
            
            //Pause events of main sequence
            _defaultEventTimelineParser.PauseEventSequence();

            //Add trigger event timeline parser
            if (_triggerEventTimelineParser == null)
            {
                _triggerEventTimelineParser = gameObject.AddComponent<EventTimelineParser>();
            }
            
            //Start trigger event sequence
            StartCoroutine(_triggerEventTimelineParser.StartEventSequence(triggerEventSequence));
            if (triggerFocusOnPlayer)
            {
                _focusActive = true;
                _triggerGameObject = triggerGO;
            }
            yield return new WaitUntil(_triggerEventTimelineParser.IsEventSequenceFinished);
            _focusActive = false;
            
            //Reset trigger event sequence 
            if (resetTriggerSequence)
            {
                GameManager.Instance.eventSystemManager.ResetEventSequenceSceneGraph(triggerEventSequence);
            }
            
            //Resume events
            _defaultEventTimelineParser.ResumeEventSequence();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FocusOnTrigger()
        {
            var direction  = (_triggerGameObject.transform.position - gameObject.transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8);
        }
    }
}