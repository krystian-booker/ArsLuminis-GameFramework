using Cinemachine;
using EventSystem.Models;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Camera;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class ChangeVirtualCameraExecution : IEventExecution
    {
        private readonly CinemachineBrain _cinemachineBrain;
        private float _timeStarted;

        public ChangeVirtualCameraExecution(Camera primaryCamera)
        {
            Assert.IsNotNull(primaryCamera,
                $"{nameof(ChangeVirtualCameraExecution)}: Primary camera is required for all camera events.");

            _cinemachineBrain = primaryCamera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(_cinemachineBrain,
                $"{nameof(ChangeVirtualCameraExecution)}: CinemachineBrain required on primary camera.");
        }

        public void Execute(Node node)
        {
            //Cast
            var cameraNode = node as ChangeVirtualCameraNode;
            Assert.IsNotNull(cameraNode, $"{nameof(ChangeVirtualCameraExecution)}: Invalid setup on CameraNode.");
            Assert.IsNotNull(cameraNode.virtualCamera, $"{nameof(ChangeVirtualCameraExecution)}: Invalid setup on CameraNode.");

            //Update priorities
            DisableVirtualCameras();

            //Set blend states
            _cinemachineBrain.m_DefaultBlend.m_Style = cameraNode.blend;
            _cinemachineBrain.m_DefaultBlend.m_Time = cameraNode.blendTime;

            //Set active camera state
            _timeStarted = Time.time;
            cameraNode.virtualCamera.Priority = (int)CameraPriorityState.Active;
        }

        public bool IsFinished()
        {
            return (Time.time - _timeStarted >= _cinemachineBrain.m_DefaultBlend.m_Time);
        }

        private void DisableVirtualCameras()
        {
            for (var i = 0; i < CinemachineCore.Instance.VirtualCameraCount; i++)
            {
                var vcam = CinemachineCore.Instance.GetVirtualCamera(i);
                if (vcam.Priority == (int)CameraPriorityState.Active)
                {
                    vcam.Priority = (int)CameraPriorityState.Secondary;
                }
                else
                {
                    vcam.Priority = (int)CameraPriorityState.Disabled;
                }
            }
        }
    }
}