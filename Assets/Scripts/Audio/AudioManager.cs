using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public GameObject defaultAudioSourceLocation;

        private Dictionary<string, AudioSource> _activeAudioSources = new Dictionary<string, AudioSource>();

        public void ReturnToPool(AudioSource audioSource)
        {
            Destroy(audioSource);
        }

        public void StartAudioCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        public void AddActiveAudioSource(string key, AudioSource audioSource)
        {
            if (!_activeAudioSources.ContainsKey(key))
            {
                _activeAudioSources.Add(key, audioSource);
            }
            else
            {
                Debug.LogError(
                    $"{nameof(AudioManager)}: Duplicate audio node Id, an audio source with the Id '{key}' already exists.");
            }
        }

        public void StopActiveAudioSource(string key)
        {
            if (_activeAudioSources.TryGetValue(key, out var audioSource))
            {
                audioSource.Stop();
                //The event sequence will capture this source is no longer active and re-pool it.
                _activeAudioSources.Remove(key);
            }
            else
            {
                Debug.LogWarning(
                    $"{nameof(AudioManager)}: Audio source with the Id '{key}' does not exist, was it already removed?");
            }
        }
    }
}