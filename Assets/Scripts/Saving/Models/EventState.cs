
using System;
using UnityEngine;

namespace Saving.Models
{
    [Serializable]
    public class EventState
    {
        [Tooltip("Unique name to identify the event state in event sequences")]
        public string id;
        
        [TextArea]
        public string description;
        public DataType dataType;

        public object defaultValue;
        public object value;
    }
}