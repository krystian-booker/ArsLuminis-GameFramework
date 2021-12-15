using Audio;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Nodes.Audio;
using Tools;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem.Events
{
    public class StartAudioExecution : IEventExecution
    {
        private GameObject _audioSourceLocation;
        private AudioSource _audioSource;
        private StartAudioNode _audioNode;

        /// <summary>
        /// TODO: We need to create a prefab gameobject that has an audioSource on it. Create a pool of that object
        /// AddComponent every audio node is terrible
        /// </summary>
        /// <param name="node"></param>
        public void Execute(Node node)
        {
            _audioNode = node as StartAudioNode;
            Assert.IsNotNull(_audioNode, $"{nameof(StartAudioExecution)}: Invalid setup on {nameof(StartAudioExecution)}");

            _audioSourceLocation = _audioNode.audioSourceLocation != null ? _audioNode.audioSourceLocation : Systems.audioManager.defaultAudioSourceLocation;

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
                Systems.audioManager.StartAudioCoroutine(FadeAudioSource.StartFade(_audioSource, _audioNode.initialFadeDelay, _audioNode.fadeDuration,
                    _audioNode.targetVolume));
            }

            if (_audioNode.isPublic)
            {
                Systems.audioManager.AddActiveAudioSource(_audioNode.publicId, _audioSource);
            }
        }

        public bool IsFinished()
        {
            if (_audioSource.isPlaying)
                return false;
            
            Systems.audioManager.ReturnToPool(_audioSource);
            return true;
        }
    }
}