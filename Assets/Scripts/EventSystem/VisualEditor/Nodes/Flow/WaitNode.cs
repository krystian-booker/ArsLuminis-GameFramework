using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Flow
{
    public class WaitNode : FlowNode
    {
        [Tooltip("Documentation purposes only")] [TextArea]
        
        public string description;
        [SerializeField] public float delayTime;
    }
}