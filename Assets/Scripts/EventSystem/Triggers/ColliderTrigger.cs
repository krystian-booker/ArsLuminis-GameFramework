using System.Collections;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;

namespace EventSystem.Triggers
{
    /// <summary>
    /// Attach to a gameObject with a trigger collider, will be triggered on character entry
    /// </summary>
    public class ColliderTrigger : MonoBehaviour
    {
        public EventSequenceGraph triggerEventSequence;

        [Tooltip("If enabled the event sequence can only be triggered once during this session. " +
                 "If the player leaves a scene and re-enters, this will be reset. To save states permanently use state nodes")]
        public bool runOnce;

        private EventSequenceParser _triggerEventSequenceParser;
        private bool _hasTriggered;

        public IEnumerator BeginTriggerEvent()
        {
            if (triggerEventSequence == null || _hasTriggered)
                yield break;

            _hasTriggered = true;

            //Add trigger event timeline parser
            if (_triggerEventSequenceParser == null)
            {
                _triggerEventSequenceParser = gameObject.AddComponent<EventSequenceParser>();
            }

            //Start trigger event sequence
            StartCoroutine(_triggerEventSequenceParser.StartEventSequence(triggerEventSequence));
            yield return new WaitUntil(_triggerEventSequenceParser.IsEventSequenceFinished);

            if (!runOnce)
                _hasTriggered = false;
        }
    }
}