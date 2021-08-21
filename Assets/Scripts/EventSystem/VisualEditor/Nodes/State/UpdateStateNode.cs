using System;
using System.Linq;
using EventSystem.Models;
using Saving.Models;
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

        [HideInInspector] public string selectedStateId;

        public string stringValue;
        public int intValue;
        public float floatValue;
        public bool booleanValue;
        public Vector3 vector3Value;

        private void OnValidate()
        {
            var selectedState = Systems.SaveManager.gameState.states.FirstOrDefault(x => x.id == selectedStateId);
            if (selectedState != null)
            {
                switch (selectedState.dataType)
                {
                    case DataType.String:
                        intValue = 0;
                        floatValue = 0;
                        booleanValue = false;
                        vector3Value = Vector3.zero;
                        break;
                    case DataType.Integer:
                        stringValue = string.Empty;
                        floatValue = 0;
                        booleanValue = false;
                        vector3Value = Vector3.zero;
                        break;
                    case DataType.Float:
                        stringValue = string.Empty;
                        intValue = 0;
                        booleanValue = false;
                        vector3Value = Vector3.zero;
                        break;
                    case DataType.Boolean:
                        stringValue = string.Empty;
                        intValue = 0;
                        floatValue = 0;
                        vector3Value = Vector3.zero;
                        break;
                    case DataType.Vector3:
                        stringValue = string.Empty;
                        intValue = 0;
                        floatValue = 0;
                        booleanValue = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}