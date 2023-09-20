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
        [SerializeField, Required] private NavMeshAgent targetAgent;

        [Tooltip("The GameObject marking the destination")]
        [SerializeField, Required] private GameObject targetObject;
        private GizmoComponent gizmoComponent;

        [Tooltip("The stopping distance for the NavMesh agent")]
        [Range(0f, 20f), SerializeField]
        private float stoppingDistance = 1;

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
                GameObject targetParent = GameObject.Find("TargetPositions");
                if (targetParent == null)
                {
                    targetParent = new GameObject("TargetPositions");
                    targetParent.transform.position = new Vector3(0, 0, 0);
                }

                int count = 1;

                // Count similar named GameObjects already present under TargetPositions
                foreach (UnityEngine.Transform child in targetParent.transform)
                {
                    if (child.name.StartsWith(string.Format("{0} - Destination", targetAgent.name)))
                    {
                        count++;
                    }
                }

                // Create GameObject with incremented name.
                var newTarget = new GameObject(string.Format("{0} - Destination {1}", targetAgent.name, count));

                gizmoComponent = newTarget.AddComponent<GizmoComponent>();
                gizmoComponent.gizmoRadius = stoppingDistance;

                newTarget.transform.position = targetAgent.transform.position;
                newTarget.transform.SetParent(targetParent.transform);

                targetObject = newTarget;
            }
            else
            {
                Debug.LogError("SetDestinationNode: Can't create a Target Object because the Target Agent has not been assigned.");
            }
        }
    }
}
