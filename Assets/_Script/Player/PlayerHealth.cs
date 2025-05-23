using System;
using _Script.Player;
using _Script.Utils;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnduranceSlider _enduranceSlider;
    private IInput _input => _playerController.input;
    [Header("Settings")]
    [SerializeField] private float maxHealthPoint = 50;
    [HideInInspector] public float MaxHealthPoint => maxHealthPoint;
    private EnduranceSystem _enduranceSystem ;
    
    [BoxGroup("CurrentHealthPoint")]
    [HideLabel]
    [ProgressBar("Min", "Max",r: 0, g: 100, b: 0, Height = 30)]
    [SerializeField] private float currentHealthPoint => _enduranceSystem.CurrentEndurance;
    [HideInInspector] public float CurrentHealthPoint => currentHealthPoint;

    [BoxGroup("CurrentHealthPoint")]
    private float Min = 0;

    [BoxGroup("CurrentHealthPoint")]
    private float Max => maxHealthPoint;

    private void Awake()
    {
        _enduranceSystem = new EnduranceSystem(maxHealthPoint, 0, 0,0);
        _enduranceSlider.Initialization(_enduranceSystem);
    }

    public void TakeDamage(float damageCount)
    {
        _enduranceSystem.RemoveValue(damageCount);
        if (currentHealthPoint <= 0)
        {
            Debug.Log("Ты умер");   
        }
    }
    
    public void AddHealth(float heathCount)
    {
        _enduranceSystem.AddValue(heathCount);
    }
}


public interface IDamageable
{
    public void TakeDamage(float damageCount);
}
