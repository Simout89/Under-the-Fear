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
    [SerializeField] private float MaxHealthPoint = 50;
    
    [BoxGroup("CurrentHealthPoint")]
    [HideLabel]
    [ProgressBar("Min", "Max",r: 0, g: 100, b: 0, Height = 30)]
    [SerializeField][ReadOnly] private float currentHealthPoint;

    [BoxGroup("CurrentHealthPoint")]
    private float Min = 0;

    [BoxGroup("CurrentHealthPoint")]
    private float Max => MaxHealthPoint;

    private void Awake()
    {
        currentHealthPoint = MaxHealthPoint;
    }

    public void TakeDamage(float damageCount)
    {
        currentHealthPoint -= damageCount;
        if (currentHealthPoint <= 0)
        {
            Debug.Log("Ты умер");   
        }
    }
}


public interface IDamageable
{
    public void TakeDamage(float damageCount);
}
