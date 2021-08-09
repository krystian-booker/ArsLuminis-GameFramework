using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public GameObject defaultAudioSourceLocation;

        public void ReturnToPool(AudioSource audioSource)
        {
            Destroy(audioSource);            
        }

        public void StartAudioCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
    }
}