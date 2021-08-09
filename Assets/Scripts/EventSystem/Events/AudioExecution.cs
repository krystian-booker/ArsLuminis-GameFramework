using Audio;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Actions;
using Tools;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class AudioExecution : IEventExecution
    {
        private GameObject _audioSourceLocation;
        private AudioSource _audioSource;
        private AudioNode _audioNode;

        /// <summary>
        /// TODO: We need to create a prefab gameobject that has an audioSource on it. Create a pool of that object
        /// AddComponent every audio node is terrible
        /// </summary>
        /// <param name="node"></param>
        public void Execute(Node node)
        {
            _audioNode = node as AudioNode;
            Assert.IsNotNull(_audioNode, $"{nameof(AudioExecution)}: Invalid setup on {nameof(AudioExecution)}");

            _audioSourceLocation = _audioNode.audioSourceLocation != null
                ? _audioNode.audioSourceLocation
                : GameManager.Instance.audioManager.defaultAudioSourceLocation;

            _audioSource = _audioSourceLocation.AddComponent<AudioSource>();
            _audioSource.clip = _audioNode.audioClip;
            _audioSource.mute = _audioNode.mute;
            _audioSource.bypassEffects = _audioNode.bypassEffects;
            _audioSource.bypassListenerEffects = _audioNode.bypassListenerEffects;
            _audioSource.bypassReverbZones = _audioNode.bypassReverbZones;
            _audioSource.loop = _audioNode.loop;
            _audioSource.priority = _audioNode.priority;
            _audioSource.volume = _audioNode.volume;
            _audioSource.pitch = _audioNode.pitch;
            _audioSource.panStereo = _audioNode.stereoPan;
            _audioSource.spatialBlend = _audioNode.spatialBlend;
            _audioSource.reverbZoneMix = _audioNode.reverbZoneMix;
            _audioSource.outputAudioMixerGroup = _audioNode.audioMixer;
            _audioSource.Play();

            if (_audioNode.audioFade)
            {
                GameManager.Instance.audioManager.StartAudioCoroutine(FadeAudioSource.StartFade(_audioSource, _audioNode.initialFadeDelay, _audioNode.fadeDuration, _audioNode.targetVolume));
            }
        }

        public bool IsFinished()
        {
            if (_audioSource.isPlaying) return false;
            GameManager.Instance.audioManager.ReturnToPool(_audioSource);
            return true;
        }
    }
}