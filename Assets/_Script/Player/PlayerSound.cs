using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _playerFlashlight;

    [Header("FlashLight Events")]
    [SerializeField] private AK.Wwise.Event flashLightEnable;
    [SerializeField] private AK.Wwise.Event flashLightDisable;
    [SerializeField] private AK.Wwise.Event flashLightBite;

    private void OnEnable()
    {
        _playerFlashlight.OnFlashLightEnable += HandleFlashLightEnable;
        _playerFlashlight.OnFlashLightDisable += HandleFlashLightDisable;
        _playerFlashlight.OnFlashLightBite += HandleFlashLightBite;
    }
    private void OnDisable()
    {
        _playerFlashlight.OnFlashLightEnable -= HandleFlashLightEnable;
        _playerFlashlight.OnFlashLightDisable -= HandleFlashLightDisable;
        _playerFlashlight.OnFlashLightBite -= HandleFlashLightBite;

    }

    private void HandleFlashLightBite()
    {
        flashLightBite.Post(gameObject);
    }

    private void HandleFlashLightEnable()
    {
        flashLightEnable.Post(gameObject);
    }
    
    private void HandleFlashLightDisable()
    {
        flashLightDisable.Post(gameObject);
    }
}
