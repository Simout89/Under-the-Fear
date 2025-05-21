using System;
using UnityEngine;
using Zenject;

public class HealStream : MonoBehaviour, IClickable, IReleasable
{
    [Header("Settings")]
    [SerializeField] private bool isFullHeal;
    [SerializeField] private float healAmount = 5;
    [Header("Sound")]
    [Inject] private PlayerHealth _playerHealth;
    [SerializeField] private AK.Wwise.Event startDrinking;
    [SerializeField] private AK.Wwise.Event stopDrinking;

    private bool drinking = false;
    public void Click()
    {
        drinking = true;
        startDrinking.Post(Camera.main.gameObject);
    }

    public void OnRelease()
    {
        if(!drinking)
            return;
        stopDrinking.Post(Camera.main.gameObject);

        drinking = false;
    }

    public void FixedUpdate()
    {
        if(!drinking)
            return;
        
        _playerHealth.AddHealth(5 * Time.deltaTime);

        if (_playerHealth.CurrentHealthPoint >= _playerHealth.MaxHealthPoint)
        {
            OnRelease();
        }
    }
}

public interface IReleasable
{
    void OnRelease();
}
