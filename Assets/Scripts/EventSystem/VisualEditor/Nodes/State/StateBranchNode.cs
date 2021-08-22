using System.Collections.Generic;
using EventSystem.Models;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State
{
    /// <summary>
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#3d5163")]
    public class StateBranchNode : BaseNode
    {
        [Input] public NodeLink entry;

        [HideInInspector] public string selectedStateId;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used.")]
        [Output(dynamicPortList = true)] public List<string> stringOptions;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used.")]
        [Output(dynamicPortList = true)] public List<int> integerOptions;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used.")]
        [Output(dynamicPortList = true)] public List<float> floatOptions;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used.")]
        [Output(dynamicPortList = true)] public List<Vector3> vector3Options;

        #region Boolean

        [Tooltip("If the selected state is true, this output will be used.")]
        [Output] public NodeLink valueTrue;

        [Tooltip("If the selected state is false, this output will be used.")]
        [Output] public NodeLink valueFalse;

        #endregion
        
        [Tooltip("If the state does not match any of the provided outputs, the default will be used")]
        [Output] public NodeLink defaultOutput;
    }
}