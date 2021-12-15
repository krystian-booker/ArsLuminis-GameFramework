using System.Collections.Generic;
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

        [Header("Event Sequencer")] 
        [Tooltip("Not required, the default event sequence that starts automatically in Start()")]
        public EventSequenceGraph defaultEventSequence;

        //Each Character has their own individual EventSequenceParser, the running EventSequence can be switched out freely
        private EventSequenceParser _defaultEventSequenceParser;

        [Header("Trigger")]
        [Tooltip("A list of GameObjects that will call the EntryTrigger if they collide.If list is empty any gameObject will will call the trigger.")]
        public List<GameObject> triggerGameObjects = new List<GameObject>();

        [Tooltip("If set to true this gameObject will turn towards the triggering game object")]
        public bool focusOnTrigger = true;

        [Tooltip("Speed that the gameObject will turn towards the triggering game object")] [Range(1f, 20f)]
        public float focusRotationSpeed = 8f;

        //Internal flag if the focus is currently active
        private bool _focusActive;

        //GameObject that triggered
        private GameObject _focusTarget;

        #endregion

        #region Character Movement Controllers

#if EASY_CHARACTER_MOVEMENT
        private AgentControllerECM _agentControllerEcm;
#else
        //If no custom character controller is being used we will default to NavMeshAgent
        private NavMeshAgent _navMeshAgent;
#endif

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
            //A default event sequence is not required, the object could run an event sequence from a trigger 
            if (defaultEventSequence != null)
            {
                _defaultEventSequenceParser = gameObject.AddComponent<EventSequenceParser>();
                StartCoroutine(_defaultEventSequenceParser.StartEventSequence(defaultEventSequence));
            }
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
            if (_defaultEventSequenceParser != null)
            {
                _defaultEventSequenceParser.PauseEventSequence();
            }
        }

        /// <summary>
        /// Used to resume an eventSequence that was paused.
        /// </summary>
        public void ResumeEventSequence()
        {
            _defaultEventSequenceParser.ResumeEventSequence();
        }

        #endregion

        #region Triggers

        /// <summary>
        /// When a trigger collides, we will call the ColliderTrigger.
        /// The current gameObject must have the tag "Trigger"
        /// If only specific collisions should trigger an event than add those GameObjects to the triggerGameObjects list.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (gameObject.CompareTag($"Trigger") && (triggerGameObjects.Count == 0 || triggerGameObjects.Contains(other.gameObject)))
            {
                var colliderTrigger = other.gameObject.GetComponent<ColliderTrigger>();
                if (Systems.DebugWarnings && colliderTrigger == null)
                {
                    Debug.LogWarning($"{nameof(CharacterManager)}: OnTriggerEnter called for '{gameObject.name}' but no ColliderTrigger component exists");
                    return;
                }

                StartCoroutine(colliderTrigger.BeginTriggerEvent());
            }
        }

        /// <summary>
        /// When a trigger collides, we will call the ColliderTrigger.
        /// The current gameObject must have the tag "Trigger"
        /// If only specific collisions should trigger an event than add those GameObjects to the triggerGameObjects list.
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer($"Character") || other.gameObject.CompareTag($"NPC"))
            {
                //InputManager: Wait for user to click 'Confirm'
                if (Systems.inputManager.onConfirmValue.started)
                {
                    var npcEventTrigger = other.gameObject.GetComponent<NpcEventTrigger>();
                    if (Systems.DebugWarnings && npcEventTrigger == null)
                    {
                        Debug.LogWarning($"{nameof(CharacterManager)}: OnTriggerStay called for '{gameObject.name}' but no NpcEventTrigger component exists");
                        return;
                    }

                    StartCoroutine(npcEventTrigger.BeginTriggerEvent(gameObject, this));
                }
            }
        }

        #endregion

        #region Focus

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
        public void RemoveFocus()
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
            if (focusOnTrigger && _focusActive && _focusTarget != null)
            {
                var direction = (_focusTarget.transform.position - gameObject.transform.position).normalized;
                var lookRotation = Quaternion.LookRotation(direction);
                gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * focusRotationSpeed);
            }
        }

        #endregion

        #region Locomotion

        public void Move(Vector3 inputMovement)
        {
#if EASY_CHARACTER_MOVEMENT
            _agentControllerEcm.moveDirection = inputMovement;
            transform.rotation = Quaternion.LookRotation(inputMovement);
#else
            _navMeshAgent.Move(inputMovement);
            transform.rotation = Quaternion.LookRotation(inputMovement);
#endif
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