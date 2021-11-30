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

        [Tooltip("Set a value here to be the default value")]
        public string stringValue;

        [Tooltip("Set a value here to be the default value")]
        public int intValue;

        [Tooltip("Set a value here to be the default value")]
        public float floatValue;

        [Tooltip("Set a value here to be the default value")]
        public bool booleanValue;

        [Tooltip("Set a value here to be the default value")]
        public Vector3 vector3Value;
    }
}