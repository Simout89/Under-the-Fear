using System;
using System.Collections;
using _Script.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class DarknessUIController: MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject icon;
        [SerializeField] private RawImage darknessIcon;
        [Header("Settings")]
        [SerializeField] private float minAlpha = 0.2f;
        [SerializeField] private float multiplayerIntensity = 10;
        private Tween blinkTween;
        private EnduranceSystem _enduranceSystem;

        private void Awake()
        {
            icon.SetActive(false);
        }
        
        public void Initialization(EnduranceSystem enduranceSystem)
        {
            _enduranceSystem = enduranceSystem;
            _enduranceSystem.OnValueChanged += HandleValueChanged;
        }

        private void OnEnable()
        {
            if(_enduranceSystem != null)
                _enduranceSystem.OnValueChanged += HandleValueChanged;
        }

        private void OnDisable()
        {
            if(_enduranceSystem != null)
                _enduranceSystem.OnValueChanged -= HandleValueChanged;
        }

        private void HandleValueChanged()
        {
            if (_enduranceSystem.CurrentEndurance == 0)
            {
                StopBlinking();
                icon.SetActive(false);
            }
            else
            {
                if (blinkTween != null)
                {
                    SetBlinkIntensity(_enduranceSystem.CurrentEndurance);
                }
                else
                {
                    icon.SetActive(true);
                    StartBlinking(_enduranceSystem.CurrentEndurance); 
                }
            }
        }
        
        public void StartBlinking(float intensity)
        {
            StopBlinking();
        
            blinkTween = darknessIcon.DOFade(minAlpha, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetSpeedBased(false);
            
            SetBlinkIntensity(intensity);
        }
    
        public void SetBlinkIntensity(float intensity)
        {
            if (blinkTween != null && blinkTween.IsActive())
            {
                blinkTween.timeScale = intensity / multiplayerIntensity;
            }
        }
    
        public void StopBlinking()
        {
            if (blinkTween != null)
            {
                blinkTween.Kill();
                blinkTween = null;
            }
        }
    }
}