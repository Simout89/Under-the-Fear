using System;
using _Script.Player;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    private IInput _input => _playerController.input;
    [Header("Settings")]
    [SerializeField] private float MaxHealthPoint = 50;
    [ReadOnly][SerializeField] private float currentHealthPoint;

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
