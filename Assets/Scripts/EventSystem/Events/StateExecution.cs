using System.Collections;
using EventSystem.Models;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Actions;
using XNode;

namespace EventSystem.Events
{
    public class StateExecution : IEventExecution
    {
        private StateNode _stateNode;

        public IEnumerator Execute(Node node)
        {
            _stateNode = node as StateNode;
            if (_stateNode != null)
            {
                var eventState = new EventStates();
                var eventStateProps = typeof(EventStates).GetProperty(_stateNode.eventState);
                if (eventStateProps != null)
                {
                    var results = eventStateProps.GetValue(eventState, null);
                }
            }
            yield return null;
        }

        public bool IsFinished()
        {
            return true;
            //throw new System.NotImplementedException();
        }
    }
}