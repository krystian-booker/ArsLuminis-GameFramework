using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Locomotion
{
    /// <summary>
    /// Able to be used for any object
    /// If you want a more detailed movement control over an object I'd recommend
    /// using CharacterMovement over ObjectMovement
    ///
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#4D908E")]
    public class ObjectMovementNode : SkippableBaseNode
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

        [Tooltip("Override the game objects current position")]
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
    }
}
