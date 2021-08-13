using System.Collections;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;

namespace EventSystem.Triggers
{
    /// <summary>
    /// Attach to a gameObject with a trigger collider, will be triggered on Player character entry
    /// </summary>
    public class EntryTrigger : MonoBehaviour
    {
        public EventSequenceSceneGraph triggerEventSequence;

        [Tooltip("If enabled the event sequence can only be triggered once during this session")]
        // If the player leaves a scene and re-enters, this will be reset
        // To save states permanently use state nodes
        public bool runOnce;
        
        private EventTimelineParser _triggerEventTimelineParser;
        private bool _hasTriggered;

        public IEnumerator BeginTriggerEvent()
        {
            if (triggerEventSequence == null || _hasTriggered)
                yield break;

            _hasTriggered = true;

            //Add trigger event timeline parser
            if (_triggerEventTimelineParser == null)
            {
                _triggerEventTimelineParser = gameObject.AddComponent<EventTimelineParser>();
            }

            //Start trigger event sequence
            StartCoroutine(_triggerEventTimelineParser.StartEventSequence(triggerEventSequence));
            yield return new WaitUntil(_triggerEventTimelineParser.IsEventSequenceFinished);
            
            if(!runOnce)
                _hasTriggered = false; 
        }
    }
}