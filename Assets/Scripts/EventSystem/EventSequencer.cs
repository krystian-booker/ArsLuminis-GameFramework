using System;
using System.Collections;
using System.Collections.Generic;
using EventSystem.Events;
using EventSystem.Events.Models;
using EventSystem.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using EventType = EventSystem.Models.EventType;

namespace EventSystem
{
    public class EventSequencer : SerializedMonoBehaviour
    {
        [Tooltip("Only required if the event sequence will be altering the camera state.")]
        public Camera primaryCamera;

        [Tooltip("Loops all events from beginning to end")]
        public bool isLoop;

        [Tooltip("List of events to occur, in order displayed here")] [OdinSerialize]
        public List<GameEvent> events = new List<GameEvent>();

        //Event cycle
        private GameEvent _currentEvent;
        private IEnumerator _currentEventCoroutine;
        private int _eventIterator;

        //Event executions
        private ScriptedCameraExecution _scriptedCameraExecution;
        private ObjectMovementExecution _objectMovementExecution;
        private CharacterMovementExecution _characterMovementExecution;

        private void Update()
        {
            //No event currently running, grab the next event.
            if (_currentEvent == null || _currentEvent.isFinished)
            {
                //Event sequence is finished
                if (_eventIterator >= events.Count)
                {
                    if (isLoop)
                        ResetStatesForSequenceLoop();
                    return;
                }

                _currentEvent = events[_eventIterator];
            }

            //Debugging state
            if (_currentEvent.skip)
            {
                _eventIterator++;
                _currentEvent.isFinished = true;
                return;
            }

            //Event is currently running
            if (_currentEvent.isStarted)
            {
                EventFinished();
                return;
            }

            StartEvent();
        }

        private void StartEvent()
        {
            if (_currentEvent.isStarted)
                return;
            
            _eventIterator++;
            _currentEvent.isStarted = true;
            switch (_currentEvent.eventType)
            {
                case EventType.Dialog:
                    break;
                case EventType.Camera:
                {
                    _scriptedCameraExecution = new ScriptedCameraExecution(primaryCamera);
                    _currentEventCoroutine = _scriptedCameraExecution.Execute(_currentEvent);
                    break;
                }
                case EventType.CharacterMovement:
                {
                    _characterMovementExecution = new CharacterMovementExecution();
                    _currentEventCoroutine = _characterMovementExecution.Execute(_currentEvent);
                    break;
                }
                case EventType.ObjectMovement:
                {
                    _objectMovementExecution = new ObjectMovementExecution();
                    _currentEventCoroutine = _objectMovementExecution.Execute(_currentEvent);
                    break;
                }
                case EventType.Parallel:
                    //TODO: Figure this out before its too late! :(
                    //new eventSequencer? 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartCoroutine(_currentEventCoroutine);
        }

        private void EventFinished()
        {
            if(_currentEvent.isFinished)
                return;

            switch (_currentEvent.eventType)
            {
                case EventType.Dialog:
                    throw new NotImplementedException();
                case EventType.Camera:
                {
                    if (_scriptedCameraExecution != null)
                    {
                        _currentEvent.isFinished = _scriptedCameraExecution.IsFinished();
                        if (_currentEvent.isFinished)
                        {
                            _scriptedCameraExecution = null;
                            GC.Collect();
                        }
                    }

                    break;
                }
                case EventType.CharacterMovement:
                {
                    if (_characterMovementExecution != null)
                    {
                        _currentEvent.isFinished = _characterMovementExecution.IsFinished();
                        if (_currentEvent.isFinished)
                        {
                            _characterMovementExecution = null;
                            GC.Collect();
                        }
                    }

                    break;
                }
                case EventType.ObjectMovement:
                {
                    if (_objectMovementExecution != null)
                    {
                        _currentEvent.isFinished = _objectMovementExecution.IsFinished();
                        if (_currentEvent.isFinished)
                        {
                            _objectMovementExecution = null;
                            GC.Collect();
                        }
                    }

                    break;
                }
                case EventType.Parallel:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ResetStatesForSequenceLoop()
        {
            _currentEvent = null;
            _eventIterator = 0;

            foreach (var ev in events)
            {
                ev.isFinished = false;
                ev.isStarted = false;
            }
        }
    }
}