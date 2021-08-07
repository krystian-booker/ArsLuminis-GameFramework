using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public GameObject defaultAudioSourceLocation;

        public void ReturnToPool(AudioSource audioSource)
        {
            Destroy(audioSource);            
        }
    }
}