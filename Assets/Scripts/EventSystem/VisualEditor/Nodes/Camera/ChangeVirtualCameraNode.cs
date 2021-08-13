using Cinemachine;
using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Camera
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#218c74")]
    public class ChangeVirtualCameraNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [TextArea, Tooltip("Documentation purposes only")]
        public string description;
        
        [Tooltip("Virtual camera to switch to")]
        public CinemachineVirtualCamera virtualCamera;

        [Tooltip("Note: Custom does not work here, you would need to create it as a ScriptableObject")]
        public CinemachineBlendDefinition.Style blend;

        [Tooltip("Time to blend, default is 2")]
        public float blendTime = 2;
    }
}