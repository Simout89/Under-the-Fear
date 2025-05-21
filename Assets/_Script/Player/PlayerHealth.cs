using System;
using _Script.Player;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    private IInput _input => _playerController.input;
    [Header("Settings")]
    [SerializeField] private float maxHealthPoint = 50;
    [HideInInspector] public float MaxHealthPoint => maxHealthPoint;
    
    [BoxGroup("CurrentHealthPoint")]
    [HideLabel]
    [ProgressBar("Min", "Max",r: 0, g: 100, b: 0, Height = 30)]
    [SerializeField] private float currentHealthPoint;
    [HideInInspector] public float CurrentHealthPoint => currentHealthPoint;

    [BoxGroup("CurrentHealthPoint")]
    private float Min = 0;

    [BoxGroup("CurrentHealthPoint")]
    private float Max => maxHealthPoint;

    private void Awake()
    {
        currentHealthPoint = maxHealthPoint;
    }

    public void TakeDamage(float damageCount)
    {
        currentHealthPoint -= damageCount;
        if (currentHealthPoint <= 0)
        {
            Debug.Log("Ты умер");   
        }
    }
    
    public void AddHealth(float heathCount)
    {
        currentHealthPoint = Math.Clamp(currentHealthPoint + heathCount, 0, maxHealthPoint);
    }
}


public interface IDamageable
{
    public void TakeDamage(float damageCount);
}
