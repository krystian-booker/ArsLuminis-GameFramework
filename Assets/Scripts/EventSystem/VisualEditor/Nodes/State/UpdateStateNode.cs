using System.Linq;
using EventSystem.Models;
using Tools;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#3d5163")]
    public class UpdateStateNode : SkippableBaseNode
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        //Used for the editor
        [HideInInspector] public string[] stateNames;
        [HideInInspector] public int selectedStateIndex;

        public string stringValue;
        public int intValue;
        public float floatValue;
        public bool booleanValue;
        public Vector3 vector3Value;

        private void OnValidate()
        {
            stateNames = Systems.SaveManager.gameState.states.Select(x => x.id).ToArray();
        }
    }
}