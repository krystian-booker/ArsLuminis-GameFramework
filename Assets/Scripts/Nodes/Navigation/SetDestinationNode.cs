using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using Assets.Scripts.Nodes;
using Assets.Scripts.Components;
using UnityEngine.Assertions;

namespace Nodes.Navigation
{
    [NodeTint(100, 100, 200)]
    public class SetDestinationNode : ExecutableNode
    {
        [Tooltip("The NavMesh agent to be moved")]
        [SerializeField] private NavMeshAgent targetAgent;

        [Tooltip("The GameObject marking the destination")]
        [SerializeField] private GameObject targetObject;
        private GizmoComponent gizmoComponent;

        [Tooltip("The stopping distance for the NavMesh agent")]
        [Range(0f, 20f), SerializeField]
        private float stoppingDistance;

        [Tooltip("Tolerance for reaching the destination")]
        [SerializeField] private float tolerance = 0.1f;

        // Update gizmoRadius when stoppingDistance is modified in the Inspector
        private void OnValidate()
        {
            if (targetObject != null && gizmoComponent == null)
            {
                gizmoComponent = targetObject.GetComponent<GizmoComponent>();
            }

            if (gizmoComponent != null)
            {
                gizmoComponent.gizmoRadius = stoppingDistance;
            }
        }

        public override void Execute()
        {
            Assert.IsNotNull(targetAgent, "Target NavMeshAgent is null. Cannot set destination.");
            Assert.IsNotNull(targetObject, "Target GameObject is null. Cannot set destination.");

            Vector3 targetPosition = targetObject.transform.position;
            targetAgent.SetDestination(targetPosition);
            targetAgent.stoppingDistance = stoppingDistance;

        }

        public override bool IsFinished()
        {
            if (targetAgent != null && targetObject != null)
            {
                float distanceToTarget = Vector3.Distance(targetAgent.transform.position, targetObject.transform.position);
                return distanceToTarget <= stoppingDistance + tolerance;
            }
            return true;
        }

        [Button("Create Target Object")]
        private void CreateTargetObject()
        {
            if (targetAgent != null)
            {
                GameObject newTarget = new GameObject(string.Format("{0} - Destination", targetAgent.name));
                gizmoComponent = newTarget.AddComponent<GizmoComponent>();
                gizmoComponent.gizmoRadius = stoppingDistance;

                newTarget.transform.position = targetAgent.transform.position;
                targetObject = newTarget;
            }
        }
    }
}
