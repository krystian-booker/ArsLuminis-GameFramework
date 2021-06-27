using Cinemachine;
using EditorTools;
using EventSystem.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    /// <summary>
    /// I'm not a cinemachine expert, I expect this class will need to be modified heavily
    ///
    /// DO NOT PUT ANY CODE HERE, WITH THE EXCEPTION OF EDITOR CODE
    /// </summary>
    [NodeTint("#43AA8B")]
    public class CameraNode : BaseNodeExtended
    {
        [Input] public NodeLink entry;
        [Output] public NodeLink exit;

        [Tooltip("Documentation purposes only")] [TextArea]
        public string description;
        
        [OdinSerialize] public CinemachineVirtualCamera virtualCamera;

        [OdinSerialize] [Tooltip("Note: Custom does not work here, you would need to create it as a ScriptableObject")]
        public CinemachineBlendDefinition.Style blend;

        [OdinSerialize] [Tooltip("Time to blend, default is 2")]
        public float blendTime = 2;

#if UNITY_EDITOR
        //TODO: Remove 'Button', create custom UI 
        [Button("Create Virtual Cam")]
        private void GenerateNewVirtualCamera()
        {
            var vcamGameObject = UnityEngine.Resources.Load<GameObject>("Prefabs/editorTools/CM vcam");
            if (vcamGameObject == null) return;

            //Assign object back to self
            var instantiatedVcam = Tools.InstantiateObject(vcamGameObject);
            virtualCamera = instantiatedVcam.GetComponent<CinemachineVirtualCamera>();
        }
#endif
    }
}