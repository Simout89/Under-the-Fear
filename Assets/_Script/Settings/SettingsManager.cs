using System;
using UnityEngine;

namespace _Script.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private void Awake()
        {
            SettingConfig settingConfig = new SettingConfig();
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