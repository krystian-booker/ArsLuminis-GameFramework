using System;

namespace Models.Saving
{
    [Serializable]
    public class EventStateValue
    {
        public EventStates name;
        public bool complete;
    }
}