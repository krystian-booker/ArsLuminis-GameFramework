using System;

namespace Saving.Models
{
    [Serializable]
    public class EventStateValue
    {
        public EventStates name;
        public bool complete;
    }
}