using System;
using _Script.Utils;
using _Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

public class DarknessObserver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _flashlight;
    [SerializeField] private PlayerHealth _playerHealth;
    [Header("Settings")]
    [SerializeField] private float maxValue = 50;
    [SerializeField] private float minValue = 0;
    [SerializeField] private float rateOfDecrease = 5;
    [SerializeField] private float rateOfIncrease = 5;
    [SerializeField] private DarknessUIController _darknessUIController;
    private EnduranceSystem endurenceSystem;
    public EnduranceSystem EnduranceSystem => endurenceSystem;
    
    [BoxGroup("DarknessCurrentCapacity")]
    [HideLabel]
    [ProgressBar("Min", "Max",r: 0, g: 0, b: 255, Height = 30)]
    [ReadOnly][SerializeField] public float CurrentCapacity;
    
    [BoxGroup("CurrentHealthPoint")]
    private float Min => minValue;

    [BoxGroup("CurrentHealthPoint")]
    public float Max => maxValue;
    
    private bool isStayInLight = false;

    private void Awake()
    {
        endurenceSystem = new EnduranceSystem(maxValue,minValue, rateOfDecrease, rateOfIncrease, minValue);
        if (_darknessUIController != null)
        {
            _darknessUIController.Initialization(endurenceSystem);
        }
    }

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
        CurrentCapacity = endurenceSystem.CurrentEndurance;
        
        if(isStayInLight || _flashlight.isFlashLightEnable)
        {
            endurenceSystem.ReduceEndurance();
            return;
        }


        if (endurenceSystem.CurrentEndurance >= maxValue)
        {
            _playerHealth.TakeDamage(99999);
            endurenceSystem.SetValue(0);
        }
        else
        {
            endurenceSystem.AddEndurance();
        }
    }
}
