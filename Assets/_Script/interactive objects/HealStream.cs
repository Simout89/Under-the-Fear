using UnityEngine;
using Zenject;

public class HealStream : MonoBehaviour, IClickable
{
    [Header("Settings")]
    [SerializeField] private bool isFullHeal;
    [SerializeField] private float healAmount = 5;
    [Header("Sound")]
    [Inject] private PlayerHealth _playerHealth;
    [SerializeField] private AK.Wwise.Event startDrinking;
    [SerializeField] private AK.Wwise.Event stopDrinking;
    public void Click()
    {
        if (isFullHeal)
        {
            if (_playerHealth.CurrentHealthPoint >= _playerHealth.MaxHealthPoint)
            {
                stopDrinking.Post(Camera.main.gameObject);

            }
            else
            {
                startDrinking.Post(Camera.main.gameObject);
                _playerHealth.AddHealth(healAmount);

            }
        }
        else
        {
            if (_playerHealth.CurrentHealthPoint >= _playerHealth.MaxHealthPoint / 50)
            {
                stopDrinking.Post(Camera.main.gameObject);
            }
            else
            {
                startDrinking.Post(Camera.main.gameObject);
                _playerHealth.AddHealth(healAmount);
            }
        }
    }
}
