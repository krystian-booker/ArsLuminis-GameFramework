using Assets.Scripts.Nodes;
using System;
using UnityEngine.Assertions;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Nodes.Sound
{
    [NodeTint(200, 100, 100)]
    public class PlaySoundNode : ExecutableNode
    {
        [Tooltip("The AudioSource to play the sound on")]
        [SerializeField, Required] private AudioSource audioSource;

        [Tooltip("The AudioClip to play")]
        [SerializeField, Required] private AudioClip audioClip;

        [Tooltip("The volume to play the sound at")]
        [Range(0f, 1f), SerializeField] private float volume = 1.0f;

        [Tooltip("The pitch to play the sound at")]
        [Range(0.1f, 3f), SerializeField] private float pitch = 1.0f;

        [Tooltip("Should the node finish immediately after starting the sound?")]
        [SerializeField] private bool finishImmediately = false;

        private float duration;
        private float startTime;

        public override void Execute()
        {
            Assert.IsNotNull(audioSource, "AudioSource is null. Cannot play sound.");
            Assert.IsNotNull(audioClip, "AudioClip is null. Cannot play sound.");

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();

            startTime = Time.time;
            duration = audioClip.length / pitch;  // Taking into account the pitch change
        }

        public override bool IsFinished()
        {
            if (finishImmediately)
            {
                return true;
            }

            if (audioSource != null && audioClip != null)
            {
                return Time.time >= startTime + duration;
            }
            return true;
        }
    }
}