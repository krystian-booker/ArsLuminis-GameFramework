using EventSystem;
using EventSystem.Triggers;
using EventSystem.VisualEditor.Graphs;
using Tools;
using UnityEngine;

#if EASY_CHARACTER_MOVEMENT
using Integrations.EasyCharacterMovement;
#else
using UnityEngine.AI;
#endif

namespace Characters
{
    /// <summary>
    /// A 'character' is any entity that needs to run a EventSequence or have controlled locomotion
    /// AI or a playable entity.
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        #region Properties
        
        [Tooltip("Not required, the event sequence will automatically be started in Start()")]
        public EventSequenceSceneGraph defaultEventSequence;

        private EventTimelineParser _defaultEventTimelineParser;

        [Tooltip("If set to true this gameObject will turn towards the triggering game object (player)")]
        public bool focusOnTrigger = true;

        [Range(1f, 20f)] public float focusRotationSpeed = 8f;
        private bool _focusActive;
        private GameObject _focusTarget;
        
        #region Character Movement Controllers

#if EASY_CHARACTER_MOVEMENT
        private AgentControllerECM _agentControllerEcm;
#else
        private NavMeshAgent _navMeshAgent;
#endif

        #endregion

        #endregion

        #region Monobehaviour
        
        private void Awake()
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm = GetComponent<AgentControllerECM>();
#else
            _navMeshAgent = GetComponent<NavMeshAgent>();
#endif
        }

        private void Start()
        {
            //Everything that moves needs a character manager, NPCs and Playable characters,
            //Not everything will need to always have a default sequence
            if (defaultEventSequence == null)
                return;

            _defaultEventTimelineParser = gameObject.AddComponent<EventTimelineParser>();
            StartCoroutine(_defaultEventTimelineParser.StartEventSequence(defaultEventSequence));
        }

        private void Update()
        {
            FocusOnTrigger();
        }

        #endregion
        
        #region EventSequence
        
        /// <summary>
        /// Used to pause an active eventSequence.
        /// Only specific events can be paused that implement the IPauseEventExecution interface
        /// </summary>
        public void PauseEventSequence()
        {
            if (_defaultEventTimelineParser != null)
            {
                _defaultEventTimelineParser.PauseEventSequence();
            }
        }

        /// <summary>
        /// Used to resume an eventSequence that was paused.
        /// </summary>
        public void ResumeEventSequence()
        {
            _defaultEventTimelineParser.ResumeEventSequence();
        }

        #endregion
        
        #region Triggers
        
        /// <summary>
        /// Used to trigger event sequences.
        /// Trigger gameObjects must have the tag 'Trigger' and have the script 'EntryTrigger' attached.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            //Only want events triggered on active player
            //TODO: We will want to extend this, I can think of scenarios where you would want an NPC to trigger an event.
            if (Systems.GameManager.activePlayer == null ||
                Systems.GameManager.activePlayer.GetInstanceID() != gameObject.GetInstanceID()) return;

            //TODO: Remove CompareTag, replace of enum for both
            if (other.gameObject.CompareTag($"Trigger"))
            {
                var entryTrigger = other.gameObject.GetComponent<EntryTrigger>();
                StartCoroutine(entryTrigger.BeginTriggerEvent());
            }
        }

        /// <summary>
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            //Only want events triggered on active player
            if (Systems.GameManager.activePlayer == null ||
                Systems.GameManager.activePlayer.GetInstanceID() != gameObject.GetInstanceID()) return;

            //TODO: Remove both nameToLayer and CompareTag, replace of enum for both
            if (other.gameObject.layer != LayerMask.NameToLayer($"Character") ||
                !other.gameObject.CompareTag($"NPC")) return;

            //InputManager wait for a confirm
            if (!Systems.InputManager.onConfirmValue.started) return;

            var npcEventTrigger = other.gameObject.GetComponent<NpcEventTrigger>();
            StartCoroutine(npcEventTrigger.BeginTriggerEvent(gameObject, this));
        }
        
        /// <summary>
        /// Sets the target this gameObject will rotate to look at until LoseFocus()
        /// is called
        /// </summary>
        /// <param name="focusTarget"></param>
        public void SetFocus(GameObject focusTarget)
        {
            _focusActive = true;
            _focusTarget = focusTarget;
        }

        /// <summary>
        /// Stop the focus event, removes focus target
        /// </summary>
        public void LoseFocus()
        {
            _focusActive = false;
            _focusTarget = null;
        }

        /// <summary>
        /// Called from update, used to keep the 'Focus' on a target set with SetFocus()
        /// Until LoseFocus() is called, this gameObject will turn to look at the focus object
        /// </summary>
        private void FocusOnTrigger()
        {
            if (!focusOnTrigger || !_focusActive || _focusTarget == null) return;
            var direction = (_focusTarget.transform.position - gameObject.transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            gameObject.transform.rotation =
                Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * focusRotationSpeed);
        }
        
        #endregion
        
        #region Locomotion

        public void Move(Vector3 inputMovement)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.moveDirection = inputMovement;
            // _agentControllerEcm.agent.Move(inputMovement * 0.5f);
#else
            _navMeshAgent.Move(inputMovement * 0.5f);
#endif
            transform.rotation = Quaternion.LookRotation(inputMovement);
        }

        public void SetDestination(Vector3 destination)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.agent.SetDestination(destination);
#else
            _navMeshAgent.SetDestination(destination);
#endif
        }

        public void Warp(Vector3 destination)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.agent.Warp(destination);
#else
            _navMeshAgent.Warp(destination);
#endif
        }

        public void SetSpeed(float speed)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.speed = speed;
#else
            _navMeshAgent.speed = speed;
#endif
        }

        public void UpdateRotation(bool updateRotation)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.agent.updateRotation = updateRotation;
#else
            _navMeshAgent.updateRotation = updateRotation;
#endif
        }

        public void SetRadius(float radius)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.agent.radius = radius;
#else
            _navMeshAgent.radius = radius;
#endif
        }

        public float GetRemainingDistance()
        {
#if EASY_CHARACTER_MOVEMENT
            return _agentControllerEcm.agent.remainingDistance;
#else
            return _navMeshAgent.remainingDistance;
#endif
        }

        public float GetStoppingDistance()
        {
#if EASY_CHARACTER_MOVEMENT
            return _agentControllerEcm.agent.stoppingDistance;
#else
            return _navMeshAgent.stoppingDistance;
#endif
        }

        public bool HasPath()
        {
#if EASY_CHARACTER_MOVEMENT
            return _agentControllerEcm.agent.hasPath;
#else
            return _navMeshAgent.hasPath;
#endif
        }

        public void IsStopped(bool stop)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.agent.isStopped = stop;
#else
            _navMeshAgent.isStopped = stop;
#endif
        }
        
        #endregion
    }
}