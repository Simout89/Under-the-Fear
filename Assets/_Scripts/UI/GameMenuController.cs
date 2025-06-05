using System;
using _Script.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class GameMenuController : MonoBehaviour
{
    [Inject] private SceneLoaderManager _sceneLoader;
    [Inject] private PlayerSettings _playerSettings;
    [Inject] private SceneLoaderManager _sceneLoaderManager;
    
    [Header("References")]
    [SerializeField] private Slider _sliderSensitivity;
    [SerializeField] private Slider _sliderMaster;
    [SerializeField] private Slider _sliderEffect;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Toggle _toggleFps;
    [SerializeField] private GameObject FPSCounter;
    
    private bool _isInitialized = false;

    public void ExitButton()
    {
        _sceneLoader.LoadScene(0);
    }

    private void OnEnable()
    {
        _sceneLoaderManager.OnSceneLoaded += HandleSceneLoaded;
        
        // Подписываемся на события изменения настроек
        if (_playerSettings != null)
        {
            _playerSettings.OnMouseSensitivityChanged += OnMouseSensitivityChanged;
            _playerSettings.OnMasterVolumeChanged += OnMasterVolumeChanged;
            _playerSettings.OnEffectVolumeChanged += OnEffectVolumeChanged;
            _playerSettings.OnMusicVolumeChanged += OnMusicVolumeChanged;
            _playerSettings.OnShowFpsChanged += OnShowFpsChanged;
        }
    }

    private void OnDisable()
    {
        _sceneLoaderManager.OnSceneLoaded -= HandleSceneLoaded;
        
        // Отписываемся от событий
        if (_playerSettings != null)
        {
            _playerSettings.OnMouseSensitivityChanged -= OnMouseSensitivityChanged;
            _playerSettings.OnMasterVolumeChanged -= OnMasterVolumeChanged;
            _playerSettings.OnEffectVolumeChanged -= OnEffectVolumeChanged;
            _playerSettings.OnMusicVolumeChanged -= OnMusicVolumeChanged;
            _playerSettings.OnShowFpsChanged -= OnShowFpsChanged;
        }
    }

    private void Start()
    {
        InitializeUI();
    }

    private void HandleSceneLoaded()
    {
        // Переинициализируем UI при загрузке новой сцены
        if (_isInitialized)
        {
            RefreshUI();
        }
    }

    private void InitializeUI()
    {
        if (_playerSettings == null || _playerSettings.SettingsConfig == null)
        {
            Debug.LogError("PlayerSettings не инициализированы!");
            return;
        }

        _isInitialized = false;

        // Инициализируем слайдеры
        InitializeSensitivitySlider();
        InitializeSoundSliders();
        InitializeFpsToggle();

        _isInitialized = true;
        Debug.Log("UI настроек инициализирован");
    }

    private void InitializeSensitivitySlider()
    {
        if (_sliderSensitivity == null) return;

        _sliderSensitivity.onValueChanged.RemoveAllListeners();
        _sliderSensitivity.value = _playerSettings.SettingsConfig.MouseSensitivity;
        _sliderSensitivity.onValueChanged.AddListener(OnSensitivitySliderChanged);
    }

    private void InitializeSoundSliders()
    {
        InitializeSoundSlider("All_Volume_Player_Settings", _sliderMaster, 
            _playerSettings.SettingsConfig.MasterVolume, OnMasterSliderChanged);
        
        InitializeSoundSlider("Volume_FX_Player_Settings", _sliderEffect, 
            _playerSettings.SettingsConfig.EffectVolume, OnEffectSliderChanged);
        
        InitializeSoundSlider("Volume_Music_Player_Settings", _sliderMusic, 
            _playerSettings.SettingsConfig.MusicVolume, OnMusicSliderChanged);
    }

    private void InitializeSoundSlider(string rtcpName, Slider slider, float settingsValue, 
        UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null) return;

        slider.onValueChanged.RemoveAllListeners();
        
        // Устанавливаем значение из настроек
        slider.value = settingsValue;
        
        // Применяем значение к звуковому движку
        AkUnitySoundEngine.SetRTPCValue(rtcpName, settingsValue);
        
        // Добавляем слушатель
        slider.onValueChanged.AddListener(callback);
    }

    private void InitializeFpsToggle()
    {
        if (_toggleFps == null) return;

        _toggleFps.onValueChanged.RemoveAllListeners();
        _toggleFps.isOn = _playerSettings.SettingsConfig.ShowFps;
        
        if (FPSCounter != null)
            FPSCounter.SetActive(_playerSettings.SettingsConfig.ShowFps);
        
        _toggleFps.onValueChanged.AddListener(OnFpsToggleChanged);
    }

    // Колбэки для слайдеров
    private void OnSensitivitySliderChanged(float value)
    {
        if (!_isInitialized) return;
        _playerSettings.SetMouseSensitivity(value);
    }

    private void OnMasterSliderChanged(float value)
    {
        if (!_isInitialized) return;
        AkUnitySoundEngine.SetRTPCValue("All_Volume_Player_Settings", value);
        _playerSettings.SetMasterVolume(value);
    }

    private void OnEffectSliderChanged(float value)
    {
        if (!_isInitialized) return;
        AkUnitySoundEngine.SetRTPCValue("Volume_FX_Player_Settings", value);
        _playerSettings.SetEffectVolume(value);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (!_isInitialized) return;
        AkUnitySoundEngine.SetRTPCValue("Volume_Music_Player_Settings", value);
        _playerSettings.SetMusicVolume(value);
    }

    private void OnFpsToggleChanged(bool value)
    {
        if (!_isInitialized) return;
        _playerSettings.SetShowFps(value);
        
        if (FPSCounter != null)
            FPSCounter.SetActive(value);
    }

    // Обработчики событий изменения настроек
    private void OnMouseSensitivityChanged(float value)
    {
        if (_sliderSensitivity != null && Math.Abs(_sliderSensitivity.value - value) > 0.01f)
            _sliderSensitivity.value = value;
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (_sliderMaster != null && Math.Abs(_sliderMaster.value - value) > 0.01f)
            _sliderMaster.value = value;
    }

    private void OnEffectVolumeChanged(float value)
    {
        if (_sliderEffect != null && Math.Abs(_sliderEffect.value - value) > 0.01f)
            _sliderEffect.value = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (_sliderMusic != null && Math.Abs(_sliderMusic.value - value) > 0.01f)
            _sliderMusic.value = value;
    }

    private void OnShowFpsChanged(bool value)
    {
        if (_toggleFps != null && _toggleFps.isOn != value)
            _toggleFps.isOn = value;
            
        if (FPSCounter != null)
            FPSCounter.SetActive(value);
    }

    private void RefreshUI()
    {
        // Обновляем UI без вызова колбэков
        _isInitialized = false;
        
        if (_sliderSensitivity != null)
            _sliderSensitivity.value = _playerSettings.SettingsConfig.MouseSensitivity;
            
        if (_sliderMaster != null)
            _sliderMaster.value = _playerSettings.SettingsConfig.MasterVolume;
            
        if (_sliderEffect != null)
            _sliderEffect.value = _playerSettings.SettingsConfig.EffectVolume;
            
        if (_sliderMusic != null)
            _sliderMusic.value = _playerSettings.SettingsConfig.MusicVolume;
            
        if (_toggleFps != null)
            _toggleFps.isOn = _playerSettings.SettingsConfig.ShowFps;
            
        if (FPSCounter != null)
            FPSCounter.SetActive(_playerSettings.SettingsConfig.ShowFps);

        _isInitialized = true;
    }
}