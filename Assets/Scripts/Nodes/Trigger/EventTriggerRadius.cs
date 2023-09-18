using Assets.Scripts.Parsers;
using UnityEngine;

namespace Assets.Scripts.Nodes.Trigger
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(EventSequenceParser))]
    public class EventTriggerRadius : MonoBehaviour
    {
        [Tooltip("Assign your EventSequenceGraph object here")]
        [SerializeField] private EventSequenceSceneGraph _triggeredEventSequenceGraph;

        [Tooltip("Should the default TriggeredEventSequenceGraph loop while the player is in the radius?")]
        [SerializeField] private bool _loopTriggeredEvent = false;

        [Tooltip("The default event sequence will be triggered after the collision event has ended")]
        [SerializeField] private bool _startDefaultAfterCollision = false;

        private EventSequenceParser _eventSequenceParser;

        private void Start()
        {
            // Make sure the SphereCollider is set as a trigger
            GetComponent<SphereCollider>().isTrigger = true;
            _eventSequenceParser = GetComponent<EventSequenceParser>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggeredEventSequenceGraph != null && other.CompareTag("Player"))
            {
                ExecuteEventSequence();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_triggeredEventSequenceGraph != null && other.CompareTag("Player") && _startDefaultAfterCollision)
            {
                // When the player exits the radius, revert back to the default sequence without looping
                _eventSequenceParser.ExecuteGraph(loop: false);
            }
        }

        private void ExecuteEventSequence()
        {
            if (_triggeredEventSequenceGraph != null)
            {
                _eventSequenceParser.ExecuteGraph(_triggeredEventSequenceGraph, _loopTriggeredEvent, !_startDefaultAfterCollision);
            }
        }

        // Draw a red sphere in the Editor to visualize the radius
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            // Get the sphere collider on the GameObject
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
            }
        }
    }
}