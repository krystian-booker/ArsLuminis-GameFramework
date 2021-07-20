using System;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EventSystem.Triggers
{
    [RequireComponent(typeof(EventTimelineParser))]
    public class NpcEventTrigger : MonoBehaviour
    {
        public EventSequenceSceneGraph defaultEventSequence;
        public EventSequenceSceneGraph triggerEventSequence;
        private EventTimelineParser _eventTimelineParser;

        private void Awake()
        {
            Assert.IsNotNull(defaultEventSequence);
            Assert.IsNotNull(triggerEventSequence);
        }

        private void Start()
        {
            _eventTimelineParser = GetComponent<EventTimelineParser>();
            _eventTimelineParser.StartEventSequence(defaultEventSequence);
            
        }

        /**
         * We have 3 problems here:
         * 1: FIXED: This is getting called repeatedly because we currently dont require any input to trigger
         * 
         * 2: Once the trigger event sequence has been called once it can't be called again because the nodes are
         * still marked as finished. We need a method to reset the nodes
         *
         * 3: The default event sequence is still running in the background bc coroutines. We need a way to PAUSE, then
         * start the coroutine. We do not want to restart the sequence from the beginning.
         */
        public void BeginTriggerEvent()
        {
            _eventTimelineParser.StartEventSequence(triggerEventSequence);
        }
    }
}