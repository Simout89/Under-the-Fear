using System;
using UnityEngine;

public class DarknessObserver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _flashlight;
    [SerializeField] private PlayerHealth _playerHealth;
    [Header("Settings")]
    [SerializeField] private float darknessDamage = 1;
    
    private bool isStayInLight = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            isStayInLight = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            isStayInLight = false;
        }
    }
    
    

    private void Update()
    {
        if(isStayInLight || _flashlight.isFlashLightEnable)
            return;
        _playerHealth.TakeDamage(darknessDamage * Time.deltaTime);
    }
}
