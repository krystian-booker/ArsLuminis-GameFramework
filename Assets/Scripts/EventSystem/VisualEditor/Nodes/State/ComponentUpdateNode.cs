using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    public class ComponentUpdateNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        public GameObject targetGameObject;
        
        public string targetComponent;
        public string targetVariable;
    }
}