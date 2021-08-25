using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Locomotion
{
    /// <summary>
    /// Expects a navmesh agent has already been configured for our object.
    /// If you want a more detailed movement control over an object I'd recommend
    /// using CharacterMovement over ObjectMovement
    /// </summary>
    [NodeTint("#e55039")]
    public class CharacterMovementNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [TextArea, Tooltip("Documentation purposes only")]
        public string description;

        [Tooltip("Not required, will be prefixed to generated targets names")]
        public string shortName;

        [Tooltip("Gameobject that will be moved")]
        public GameObject target;

        [Tooltip("Position gameobject will be moved to")]
        public GameObject targetPosition;

        [Tooltip("Override the game objects current position")]
        public GameObject startingPosition;

        [Range(0.5f, 5), Tooltip("Radius area to target for movement to be considered finished, recommend 1f")]
        public float distanceThreshold = 1f;

        [Range(0.1f, 10), Tooltip("Speed that the object will move at, set on initialization. Default 3.5f")]
        public float speed = 3.5f;

        [Tooltip("Objects rotation will not be altered on movement")]
        public bool disableRotation;

        [Tooltip("When calculating distance from the target, only the XY values will be used. " +
                 "This is highly recommended unless you have nav mesh areas stacked on-top of each other. " +
                 "A well tuned nav-mesh agent is required as well as you're relying on Z values as well.")]
        public bool xyThresholdOnly = true;

        [Tooltip("Prints out distance from target")]
        public bool debugDistance;
    }
}