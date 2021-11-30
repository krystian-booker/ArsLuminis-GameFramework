using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem.Models;
using Saving.Models;
using Tools;
using UnityEngine;
using XNode;

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

        [Tooltip("If the selected state matches a value in the list, the matched output will be used."), Output(dynamicPortList = true)]
        public List<string> stringOptions;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used."), Output(dynamicPortList = true)]
        public List<int> integerOptions;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used."), Output(dynamicPortList = true)]
        public List<float> floatOptions;

        [Tooltip("If the selected state matches a value in the list, the matched output will be used."), Output(dynamicPortList = true)]
        public List<Vector3> vector3Options;

        #region Boolean

        [Tooltip("If the selected state is true, this output will be used.")] [Output]
        public NodeLink valueTrue;

        [Tooltip("If the selected state is false, this output will be used.")] [Output]
        public NodeLink valueFalse;

        #endregion

        [Tooltip("If the state does not match any of the provided outputs, the default will be used")] [Output]
        public NodeLink defaultOutput;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var selectedState = Systems.SaveManager.saveTemplate.states.FirstOrDefault(x => x.id == selectedStateId);
            if (selectedState != null)
            {
                //TODO: This likely needs to be removed, breaking ui (KB - 2021/08/22)
                //TODO: Why does this need to be removed? This needs to be retested, I don't remember what the issue is (KB - 2021/11/29)
                switch (selectedState.dataType)
                {
                    case DataType.String:
                        integerOptions.Clear();
                        floatOptions.Clear();
                        vector3Options.Clear();
                        break;
                    case DataType.Integer:
                        stringOptions.Clear();
                        floatOptions.Clear();
                        vector3Options.Clear();
                        break;
                    case DataType.Float:
                        stringOptions.Clear();
                        integerOptions.Clear();
                        vector3Options.Clear();
                        break;
                    case DataType.Boolean:
                        stringOptions.Clear();
                        integerOptions.Clear();
                        floatOptions.Clear();
                        vector3Options.Clear();
                        break;
                    case DataType.Vector3:
                        stringOptions.Clear();
                        integerOptions.Clear();
                        floatOptions.Clear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
#endif
    }
}