using System.Collections;
using Cinemachine;
using EventSystem.Events.interfaces;
using EventSystem.Events.Models;
using EventSystem.Models;
using UnityEngine;

namespace EventSystem.Events
{
    public class ScriptedCameraExecution : IEventExecution
    {
        private Camera _primaryCamera;
        private CinemachineBrain _cinemachineBrain;
        private float _timeStarted;
        
        public ScriptedCameraExecution(Camera primaryCamera)
        {
            if (primaryCamera == null)
            {
                Debug.LogError($"{nameof(ScriptedCameraExecution)}: Primary camera is required for all camera events.");
            }
            
            _primaryCamera = primaryCamera;
            _cinemachineBrain = primaryCamera.GetComponent<CinemachineBrain>();
            
            if (_cinemachineBrain == null)
            {
                Debug.LogError($"{nameof(ScriptedCameraExecution)}: CinemachineBrain required on primary camera.");
            }
        }
        
        public IEnumerator Execute(GameEvent gameEvent)
        {
            //Set blend states
            _cinemachineBrain.m_DefaultBlend.m_Style = gameEvent.scriptedCamera.blend;
            _cinemachineBrain.m_DefaultBlend.m_Time = gameEvent.scriptedCamera.blendTime;
            
            //Initial delay
            yield return (gameEvent.initialDelayTime > 0 ? new WaitForSeconds(gameEvent.initialDelayTime) : null);

            //Set active camera state
            _timeStarted = Time.time;
            gameEvent.scriptedCamera.virtualCamera.Priority = (int) CameraPriorityState.Active;
        }

        public bool IsFinished()
        {
            //Once the blend is finished event is finished
            //TODO: Possibly add an override for more control?
            return (Time.time - _timeStarted >= _cinemachineBrain.m_DefaultBlend.m_Time);
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}