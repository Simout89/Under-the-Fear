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
    [Header("References")]
    [SerializeField] private Slider _sliderSensitivity;
    
    public void ExitButton()
    {
        _sceneLoader.LoadScene(0);
    }

    private void Awake()
    {
        _sliderSensitivity.value = _playerSettings.SettingsConfig.MouseSensitivity;
        _sliderSensitivity.onValueChanged.AddListener(MouseSensitivity);
    }

    public void MouseSensitivity(float value)
    {
        _playerSettings.SettingsConfig.MouseSensitivity = value;
    }
}
