using Assets.Scripts.Nodes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Nodes.Transform
{
    [NodeTint(100, 100, 200)] // Choose your color
    public class UpdateTransformNode : ExecutableNode
    {
        [Tooltip("The GameObject to be moved")]
        [SerializeField, Required] private GameObject targetObject;

        [Tooltip("The new position in Vector3 format")]
        [SerializeField, Required] private Vector3 newPosition;

        public override void Execute()
        {
            Assert.IsNotNull(targetObject, "Target GameObject is null. Cannot update position.");
            targetObject.transform.position = newPosition;
        }

        public override bool IsFinished()
        {
            return true;
        }
    }
}
