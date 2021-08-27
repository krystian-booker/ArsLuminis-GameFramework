using System;
using Characters.models;
using EventSystem;
using EventSystem.Triggers;
using EventSystem.VisualEditor.Graphs;
using Tools;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterStats characterStats;

        [Range(0.1f, 0.2f)] public float playerSpeed = 0.1f;

        #region Event Sequence

        [Tooltip("Not required")] public EventSequenceSceneGraph defaultEventSequence;
        private EventTimelineParser _defaultEventTimelineParser;

        [Tooltip("If set to true this gameObject will turn towards the triggering game object (player)")]
        public bool focusOnTrigger = true;

        [Range(1f, 20f)] public float focusRotationSpeed = 8f;
        private bool _focusActive;
        private GameObject _focusTarget;

        #endregion

        #region Animations

        public Animations animations;

        [Tooltip("Disable if the 'character' does not have animations and as a result does not have an animator attached")]
        public bool characterHasAnimations = true;

        private float _activeSpeed;
        private Vector3 _lastPosition;

        #endregion

        #region Components

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        #endregion

        #region UnityEvents

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            if (characterHasAnimations)
                Assert.IsNotNull(_animator, $"{nameof(CharacterManager)}: Missing Animator on Character {gameObject.name}");
        }

        private void Start()
        {
            //Everything that moves needs a character manager, NPCs and Playable characters,
            //Not everything will need to always have a sequence attached
            if (defaultEventSequence == null)
                return;

            _defaultEventTimelineParser = gameObject.AddComponent<EventTimelineParser>();
            StartCoroutine(_defaultEventTimelineParser.StartEventSequence(defaultEventSequence));
        }

        private void Update()
        {
            FocusOnTrigger();
            UpdatePlayerSpeed();
            
            if (characterHasAnimations)
                UpdateAnimations();
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            //Only want events triggered on active player
            if (Systems.Instance.gameManager.activePlayer == null ||
                Systems.Instance.gameManager.activePlayer.GetInstanceID() != gameObject.GetInstanceID()) return;

            //TODO: Remove both nameToLayer and CompareTag, replace of enum for both
            if (!other.gameObject.CompareTag("Trigger")) return;

            var entryTrigger = other.gameObject.GetComponent<EntryTrigger>();
            StartCoroutine(entryTrigger.BeginTriggerEvent());
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            //Only want events triggered on active player
            if (Systems.Instance.gameManager.activePlayer == null ||
                Systems.Instance.gameManager.activePlayer.GetInstanceID() != gameObject.GetInstanceID()) return;

            //TODO: Remove both nameToLayer and CompareTag, replace of enum for both
            if (other.gameObject.layer != LayerMask.NameToLayer("Character") ||
                !other.gameObject.CompareTag("NPC")) return;

            //InputManager wait for a confirm
            if (!Systems.Instance.inputManager.onConfirmValue.started) return;

            var npcEventTrigger = other.gameObject.GetComponent<NpcEventTrigger>();
            StartCoroutine(npcEventTrigger.BeginTriggerEvent(this.gameObject, this));
        }

        #endregion

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
            if (_defaultEventTimelineParser != null)
            {
                _defaultEventTimelineParser.ResumeEventSequence();
            }
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

        private void UpdatePlayerSpeed()
        {
            var curMove = transform.position - _lastPosition;
            _activeSpeed = curMove.magnitude / Time.deltaTime;
            _lastPosition = transform.position;
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

        private void UpdateAnimations()
        {
            var forward = transform.forward;
            forward.y = 0;

            var headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;

            _animator.SetFloat(animations.planarSpeedParameter, _activeSpeed);
            _animator.SetFloat(animations.rotationParameter, headingAngle);
        }
    }
}