
using System;
using UnityEngine;

namespace Saving.Models
{
    [Serializable]
    public class EventState
    {
        [Tooltip("Unique name to identify the event state in event sequences")]
        public string id;
        
        [Tooltip("Not used, documentation only")]
        public string description;
        
        [Tooltip("The type of data that you want to save")]
        public DataType dataType;

        public string defaultStringValue;
        public string stringValue;
        
        public int defaultIntValue;
        public int intValue;
        
        public float defaultFloatValue;
        public float floatValue;
        
        public bool defaultBooleanValue;
        public bool booleanValue;
        
        public Vector3 defaultVector3Value;
        public Vector3 vector3Value;
    }
}