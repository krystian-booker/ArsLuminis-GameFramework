using EventSystem.Models;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    /// <summary>
    /// Able to be used for any object
    /// If you want a more detailed movement control over an object I'd recommend
    /// using CharacterMovement over ObjectMovement
    ///
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    public class ObjectMovementNode : BaseNodeExtended
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Documentation purposes only")] [TextArea]
        public string description;
        
        [Tooltip("Not required, will be prefixed to generated targets names")]
        public string shortName;
        
        [Tooltip("Gameobject that will be moved")]
        public GameObject target;

        [Tooltip("Position gameobject will be moved to")] 
        public GameObject targetPosition;

        [Tooltip("Override the gameobjects current position")]
        public GameObject startingPosition;

        [Tooltip(
            "A buffer zone for how close the target needs to be to the targetPosition before the event is considered 'finished'")]
        [Range(0.5f, 5)]
        public float distanceThreshold = 1f;

        [Tooltip("Speed that the object will move at, set on initialization. Default 3.5f")] [Range(0.1f, 10)]
        public float speed = 3.5f;

        [Tooltip("Objects rotation will not be altered on movement")]
        public bool disableRotation;

        [Tooltip("Set the size of the navmesh radius")]
        public float navMeshRadius = 0.5f;

#if UNITY_EDITOR
        [Button("Create Target Position")]
        private void GenerateTargetPosition()
        {
            var positionTargetGameObject = Resources.Load<GameObject>("Prefabs/editorTools/YellowTarget");
            if (positionTargetGameObject == null) return;

            //Assign object back to self
            var instantiatedTarget = Utilities.InstantiateObject(positionTargetGameObject);
            instantiatedTarget.name =
                string.IsNullOrEmpty(shortName) ? "TargetPosition" : $"{shortName}TargetPosition";
            targetPosition = instantiatedTarget;
        }

        [Button("Create Starting Position")]
        private void GenerateStartingPosition()
        {
            var positionTargetGameObject = Resources.Load<GameObject>("Prefabs/editorTools/GreenTarget");
            if (positionTargetGameObject == null) return;

            //Assign object back to self
            var instantiatedTarget = Utilities.InstantiateObject(positionTargetGameObject);
            instantiatedTarget.name =
                string.IsNullOrEmpty(shortName) ? "StartingPosition" : $"{shortName}StartingPosition";
            startingPosition = instantiatedTarget;
        }
#endif
    }
}
