using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using EventType = EventSystem.Models.EventType;

namespace EventSystem.Events.Models
{
    /// <summary>
    /// Base GameEvent used for all events
    /// </summary>*
    public class GameEvent
    {
        [HideInEditorMode] public bool isStarted;

        [HideInEditorMode] public bool isFinished;

        [Tooltip("Documentation purposes only")] [TextArea]
        public string description;

        [OdinSerialize] [Tooltip("Amount of seconds that the event will be delayed before starting")] [Range(0, 20)]
        public int initialDelayTime = 0;

        [Tooltip("Skip gameEvent in sequence, helpful for debugging")]
        public bool skip;
        
        [OdinSerialize] public EventType eventType;

        //Event types
        [OdinSerialize] [ShowIf("eventType", EventType.Animation)]
        public ScriptedAnimation scriptedAnimation;
        
        [OdinSerialize] [ShowIf("eventType", EventType.Camera)]
        public ScriptedCamera scriptedCamera;
        
        [OdinSerialize] [ShowIf("eventType", EventType.CharacterMovement)]
        public CharacterMovement characterMovement;

        [OdinSerialize] [ShowIf("eventType", EventType.ObjectMovement)]
        public ObjectMovement objectMovement;

        
        // [OdinSerialize] [ShowIf("eventType", EventType.Parallel)]
        // public List<GameEvent> parallel;
    }
}