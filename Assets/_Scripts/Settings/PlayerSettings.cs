using System;
using UnityEngine;

namespace _Script.Settings
{
    public class PlayerSettings : MonoBehaviour
    {
        public SettingConfig SettingsConfig { set; get; }
        private void Awake()
        {
            SettingsConfig = new SettingConfig();
        }
    }

    public class SettingConfig
    {
        public float MouseSensitivity = 10;
        public float MasterVolume = 10;
        public float EffectVolume = 10;
        public float MusicVolume = 10;
    }
}