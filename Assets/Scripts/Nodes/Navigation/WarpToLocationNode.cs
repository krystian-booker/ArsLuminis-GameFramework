using Assets.Scripts.Components;
using Assets.Scripts.Nodes;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine;

namespace Nodes.Navigation
{
    [NodeTint(200, 100, 100)]
    public class WarpToLocationNode : ExecutableNode
    {
        [Tooltip("The NavMesh agent to be warped")]
        [SerializeField] private NavMeshAgent targetAgent;

        [Tooltip("The GameObject marking the warp location")]
        [SerializeField] private GameObject targetObject;
        private GizmoComponent gizmoComponent;

        [Tooltip("Tolerance for reaching the destination")]
        [SerializeField] private float tolerance = 0.1f;

        private void OnValidate()
        {
            if (targetObject != null && gizmoComponent == null)
            {
                gizmoComponent = targetObject.GetComponent<GizmoComponent>();
            }
        }

        public override void Execute()
        {
            Assert.IsNotNull(targetAgent, "Target NavMeshAgent is null. Cannot warp.");
            Assert.IsNotNull(targetObject, "Target GameObject is null. Cannot warp.");

            Vector3 warpPosition = targetObject.transform.position;
            targetAgent.Warp(warpPosition);
        }

        public override bool IsFinished()
        {
            if (targetAgent != null && targetObject != null)
            {
                float distanceToTarget = Vector3.Distance(targetAgent.transform.position, targetObject.transform.position);
                return distanceToTarget <= tolerance;
            }
            return true;
        }

        [Button("Create Warp Target Object")]
        private void CreateWarpTargetObject()
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
                    if (child.name.StartsWith(string.Format("{0} - Warp", targetAgent.name)))
                    {
                        count++;
                    }
                }

                // Create GameObject with incremented name.
                var newTarget = new GameObject(string.Format("{0} - Warp {1}", targetAgent.name, count));

                gizmoComponent = newTarget.AddComponent<GizmoComponent>();

                newTarget.transform.position = targetAgent.transform.position;
                newTarget.transform.SetParent(targetParent.transform);

                targetObject = newTarget;
            } else
            {
                Debug.LogError("WarpToLocationNode: Can't create a Warp Target Object because the Target Agent has not been assigned.");
            }
        }
    }
}
