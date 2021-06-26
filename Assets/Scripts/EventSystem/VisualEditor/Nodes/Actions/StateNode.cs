using System.Linq;
using EventSystem.Models;
using Saving.Models;
using Sirenix.OdinInspector;

namespace EventSystem.VisualEditor.Nodes.Actions
{
    [NodeTint("#F9844A")]
    public class StateNode : BaseNode
    {
        [Input] public NodeLink entry;
        
        // [ValueDropdown("_eventStateNames")]
        // [DisableIf("@this._eventStateNames == null || this._eventStateNames.Length == 0")]
        public EventStates eventState;

        [Output] public NodeLink stateTrue;
        [Output] public NodeLink stateFalse;
        
// #if UNITY_EDITOR
//         private string[] _eventStateNames;
//
//         [OnInspectorGUI("LoadEventStates")]
//         private void LoadEventStates()
//         {
//             //Quick instance here is fine b/c we don't care about values, only wants the names
//             _eventStateNames = new EventStates().GetType()
//                 .GetFields()
//                 .Select(field => field.Name)
//                 .ToArray();
//         }
// #endif
    }
}