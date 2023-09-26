using Assets.Scripts.Nodes;
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nodes.Flow
{
    [NodeTint(100, 0, 100)]
    public class DelayNode : ExecutableNode
    {
        [SerializeField, Required, Range(1, 60)]
        private int delayTime = 1; // Time in seconds

        private bool _isFinished;
        private Stopwatch stopwatch;

        public override void Execute()
        {
            _isFinished = false;
            stopwatch = Stopwatch.StartNew();
        }

        public override bool IsFinished()
        {
            if (!_isFinished && stopwatch != null)
            {
                if (stopwatch.Elapsed.TotalSeconds >= delayTime)
                {
                    _isFinished = true;
                    stopwatch.Stop();
                }
            }

            return _isFinished;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _isFinished = true;
            if (stopwatch != null)
            {
                stopwatch.Stop();
            }
            context.action.performed -= OnInteractPerformed;
        }
    }
}
