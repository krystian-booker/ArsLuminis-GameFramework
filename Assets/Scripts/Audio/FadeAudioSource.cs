using System.Collections;
using UnityEngine;

namespace Audio
{
    public static class FadeAudioSource {

        public static IEnumerator StartFade(AudioSource audioSource, float fadeDelay, float duration, float targetVolume)
        {
            var delayTime = 0f;
            var fadeTime = 0f;
            var initialVolume = audioSource.volume;

            while (delayTime < fadeDelay)
            {
                delayTime += Time.deltaTime;
                yield return null;
            }
            
            while (fadeTime < duration)
            {
                fadeTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, fadeTime / duration);
                Debug.Log(audioSource.volume);
                yield return null;
            }
        }
    }
}