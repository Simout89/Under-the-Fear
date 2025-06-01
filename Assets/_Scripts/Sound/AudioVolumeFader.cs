using System;
using System.Collections;
using UnityEngine;

namespace _Script.Sound
{
    [Serializable]
    public class AudioVolumeFader
    {
        [SerializeField] public AudioSource audioSource; // TODO: будет инститься
        [SerializeField] private float fadeDuration = 2.0f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
        private bool isFading = false;
        private MonoBehaviour coroutineRunner;
        private Coroutine currentFadeCoroutine;
    
        public void Init(MonoBehaviour coroutineRunner, float fadeDuration = 2.0f)
        {
            this.coroutineRunner = coroutineRunner;
            this.fadeDuration = fadeDuration;
        }
    
        public void FadeIn(float targetVolume = 1.0f)
        {
            if (isFading && currentFadeCoroutine != null)
                coroutineRunner.StopCoroutine(currentFadeCoroutine);
            
            currentFadeCoroutine = coroutineRunner.StartCoroutine(FadeVolume(0, targetVolume));
        }
    
        public void FadeOut(bool startFromCurrent = true)
        {
            if (isFading && currentFadeCoroutine != null)
                coroutineRunner.StopCoroutine(currentFadeCoroutine);
            
            float startVolume = startFromCurrent ? audioSource.volume : 1.0f;
            currentFadeCoroutine = coroutineRunner.StartCoroutine(FadeVolume(startVolume, 0));
        }
    
        public void FadeToVolume(float targetVolume)
        {
            if (isFading && currentFadeCoroutine != null)
                coroutineRunner.StopCoroutine(currentFadeCoroutine);
            
            currentFadeCoroutine = coroutineRunner.StartCoroutine(FadeVolume(audioSource.volume, targetVolume));
        }
    
        public void FadeFromTo(float fromVolume, float toVolume)
        {
            if (isFading && currentFadeCoroutine != null)
                coroutineRunner.StopCoroutine(currentFadeCoroutine);
            
            currentFadeCoroutine = coroutineRunner.StartCoroutine(FadeVolume(fromVolume, toVolume));
        }
    
        private IEnumerator FadeVolume(float startVolume, float endVolume)
        {
            isFading = true;
        
            if (!audioSource.isPlaying && endVolume > 0)
            {
                audioSource.volume = startVolume;
                audioSource.Play();
            }
        
            float timeElapsed = 0;
        
            while (timeElapsed < fadeDuration)
            {
                float progress = timeElapsed / fadeDuration;
                float evaluatedProgress = fadeCurve.Evaluate(progress);
                audioSource.volume = Mathf.Lerp(startVolume, endVolume, evaluatedProgress);
            
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        
            audioSource.volume = endVolume;
        
            if (endVolume <= 0)
            {
                audioSource.Stop();
            }
        
            isFading = false;
            currentFadeCoroutine = null;
        }
    }
}