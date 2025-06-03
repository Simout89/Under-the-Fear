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
        [SerializeField] private float multiplayerIntensity = 10f;
        
        private Tween blinkTween;
        private EnduranceSystem _enduranceSystem;
        private bool _isSubscribed = false;

        private void Awake()
        {
            icon.SetActive(false);
            
            // Валидация настроек
            if (multiplayerIntensity <= 0)
            {
                Debug.LogWarning($"[DarknessUIController] multiplayerIntensity должен быть больше 0. Установлено значение по умолчанию: 10");
                multiplayerIntensity = 10f;
            }
            
            minAlpha = Mathf.Clamp01(minAlpha);
        }
        
        public void Initialization(EnduranceSystem enduranceSystem)
        {
            // Отписываемся от предыдущей системы, если была
            if (_enduranceSystem != null && _isSubscribed)
            {
                _enduranceSystem.OnValueChanged -= HandleValueChanged;
                _isSubscribed = false;
            }
            
            _enduranceSystem = enduranceSystem;
            
            if (_enduranceSystem != null)
            {
                _enduranceSystem.OnValueChanged += HandleValueChanged;
                _isSubscribed = true;
                
                // Инициализируем состояние сразу
                HandleValueChanged();
            }
        }

        private void OnEnable()
        {
            // Подписываемся только если еще не подписаны
            if (_enduranceSystem != null && !_isSubscribed)
            {
                _enduranceSystem.OnValueChanged += HandleValueChanged;
                _isSubscribed = true;
                HandleValueChanged(); // Обновляем состояние при включении
            }
        }

        private void OnDisable()
        {
            if (_enduranceSystem != null && _isSubscribed)
            {
                _enduranceSystem.OnValueChanged -= HandleValueChanged;
                _isSubscribed = false;
            }
            
            // Останавливаем анимацию при отключении
            StopBlinking();
        }

        private void OnDestroy()
        {
            // Гарантированная очистка при уничтожении
            if (_enduranceSystem != null && _isSubscribed)
            {
                _enduranceSystem.OnValueChanged -= HandleValueChanged;
                _isSubscribed = false;
            }
            
            StopBlinking();
        }

        private void HandleValueChanged()
        {
            if (_enduranceSystem == null) return;
            
            if (_enduranceSystem.CurrentEndurance <= 0)
            {
                StopBlinking();
                icon.SetActive(false);
            }
            else
            {
                if (!icon.activeInHierarchy)
                {
                    icon.SetActive(true);
                }
                
                if (blinkTween != null && blinkTween.IsActive())
                {
                    SetBlinkIntensity(_enduranceSystem.CurrentEndurance);
                }
                else
                {
                    StartBlinking(_enduranceSystem.CurrentEndurance); 
                }
            }
        }
        
        public void StartBlinking(float intensity)
        {
            StopBlinking();
            
            if (darknessIcon == null)
            {
                Debug.LogError("[DarknessUIController] darknessIcon не назначен!");
                return;
            }
        
            // Устанавливаем начальную альфу
            Color color = darknessIcon.color;
            color.a = 1f;
            darknessIcon.color = color;
            
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
                // Защита от деления на ноль и отрицательных значений
                float timeScale = Mathf.Max(0.1f, intensity / multiplayerIntensity);
                blinkTween.timeScale = timeScale;
            }
        }
    
        public void StopBlinking()
        {
            if (blinkTween != null)
            {
                blinkTween.Kill();
                blinkTween = null;
            }
            
            // Сбрасываем альфу на полную непрозрачность
            if (darknessIcon != null)
            {
                Color color = darknessIcon.color;
                color.a = 1f;
                darknessIcon.color = color;
            }
        }
        
        // Дополнительный метод для принудительного обновления состояния
        public void ForceUpdateState()
        {
            if (_enduranceSystem != null)
            {
                HandleValueChanged();
            }
        }
    }
}