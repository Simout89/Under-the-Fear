using System;
using UnityEngine;

namespace _Script.Settings
{
    public class PlayerSettings : MonoBehaviour
    {
        [SerializeField] private SettingConfig _settingsConfig;
        
        public SettingConfig SettingsConfig 
        { 
            get => _settingsConfig;
            private set => _settingsConfig = value;
        }

        // События для уведомления об изменении настроек
        public event Action<float> OnMouseSensitivityChanged;
        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnEffectVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<bool> OnShowFpsChanged;

        private void Awake()
        {
            // Не уничтожать при загрузке новой сцены
            DontDestroyOnLoad(gameObject);
            
            LoadSettings();
        }

        private void OnDestroy()
        {
            SaveSettings();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveSettings();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SaveSettings();
        }

        public void LoadSettings()
        {
            SettingsConfig = new SettingConfig();
            
            // Загружаем настройки из PlayerPrefs
            SettingsConfig.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 10f);
            SettingsConfig.MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 10f);
            SettingsConfig.EffectVolume = PlayerPrefs.GetFloat("EffectVolume", 10f);
            SettingsConfig.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 10f);
            SettingsConfig.ShowFps = PlayerPrefs.GetInt("ShowFps", 0) == 1;
            
            Debug.Log("Настройки загружены");
        }

        public void SaveSettings()
        {
            if (SettingsConfig == null) return;
            
            // Сохраняем настройки в PlayerPrefs
            PlayerPrefs.SetFloat("MouseSensitivity", SettingsConfig.MouseSensitivity);
            PlayerPrefs.SetFloat("MasterVolume", SettingsConfig.MasterVolume);
            PlayerPrefs.SetFloat("EffectVolume", SettingsConfig.EffectVolume);
            PlayerPrefs.SetFloat("MusicVolume", SettingsConfig.MusicVolume);
            PlayerPrefs.SetInt("ShowFps", SettingsConfig.ShowFps ? 1 : 0);
            
            PlayerPrefs.Save();
            Debug.Log("Настройки сохранены");
        }

        // Методы для изменения настроек с автосохранением
        public void SetMouseSensitivity(float value)
        {
            SettingsConfig.MouseSensitivity = value;
            OnMouseSensitivityChanged?.Invoke(value);
            SaveSettings();
        }

        public void SetMasterVolume(float value)
        {
            SettingsConfig.MasterVolume = value;
            OnMasterVolumeChanged?.Invoke(value);
            SaveSettings();
        }

        public void SetEffectVolume(float value)
        {
            SettingsConfig.EffectVolume = value;
            OnEffectVolumeChanged?.Invoke(value);
            SaveSettings();
        }

        public void SetMusicVolume(float value)
        {
            SettingsConfig.MusicVolume = value;
            OnMusicVolumeChanged?.Invoke(value);
            SaveSettings();
        }

        public void SetShowFps(bool value)
        {
            SettingsConfig.ShowFps = value;
            OnShowFpsChanged?.Invoke(value);
            SaveSettings();
        }
    }

    [System.Serializable]
    public class SettingConfig
    {
        public float MouseSensitivity = 10f;
        public float MasterVolume = 100f;
        public float EffectVolume = 100f;
        public float MusicVolume = 100f;
        public bool ShowFps = false;
    }
}