using System;
using System.Collections;
using Cinemachine;
using EventSystem.Models;
using EventSystem.Models.interfaces;
using UnityEngine;
using XNode;

namespace EventSystem.Events
{
    public class CameraExecution : IEventExecution
    {
        private readonly CinemachineBrain _cinemachineBrain;
        private float _timeStarted;

        public CameraExecution(Camera primaryCamera)
        {
            if (primaryCamera == null)
            {
                Debug.LogError($"{nameof(CameraExecution)}: Primary camera is required for all camera events.");
            }

            _cinemachineBrain = primaryCamera.GetComponent<CinemachineBrain>();
            if (_cinemachineBrain == null)
            {
                Debug.LogError($"{nameof(CameraExecution)}: CinemachineBrain required on primary camera.");
            }
        }

        public IEnumerator Execute(Node node)
        {
            //Cast
            var cameraNode = node as CameraNode;
            if (cameraNode != null && cameraNode.virtualCamera != null)
            {
                //Update priorities
                DisableVirtualCameras();
                
                //Set blend states
                _cinemachineBrain.m_DefaultBlend.m_Style = cameraNode.blend;
                _cinemachineBrain.m_DefaultBlend.m_Time = cameraNode.blendTime;

                //Set active camera state
                _timeStarted = Time.time;
                cameraNode.virtualCamera.Priority = (int) CameraPriorityState.Active;
                yield return null; 
            }
            else
            {
                Debug.LogException(new Exception($"{nameof(CameraExecution)}: Invalid setup on CameraNode."));
            }
        }

        public bool IsFinished()
        {
            //TODO: Add an override time or event for more control
            //Once the blend is finished, event is finished
            return (Time.time - _timeStarted >= _cinemachineBrain.m_DefaultBlend.m_Time);
        }

        private void DisableVirtualCameras()
        {
            for (var i = 0; i < CinemachineCore.Instance.VirtualCameraCount; i++)
            {
                var vcam = CinemachineCore.Instance.GetVirtualCamera(i);
                if (vcam.Priority == (int) CameraPriorityState.Active)
                {
                    vcam.Priority = (int) CameraPriorityState.Secondary;
                }
                else
                {
                    vcam.Priority = (int) CameraPriorityState.Disabled;
                }
            }
        }
    }
}