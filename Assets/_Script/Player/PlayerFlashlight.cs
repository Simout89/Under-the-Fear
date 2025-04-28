using System;
using _Script.Player;
using UnityEngine;
using Zenject;

public class PlayerFlashlight : MonoBehaviour
{
    [Inject] private MonsterEars _monsterEars;
    
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject flashLightGameObject;
    [SerializeField] private PlayerHealth _playerHealth;

    [Header("Settings")]
    [SerializeField] private float FlashLightMaxCapacity = 50;
    [ReadOnly][SerializeField] private float flashLightCurrentCapacity;
    [SerializeField] private float rateOfDecrease = 1;
    [SerializeField] private float damageToPlayer = 10;
    [SerializeField] private float chargeRecovery = 20;

    private IInput _input => _playerController.input;

    public bool isFlashLightEnable { get; private set; }

    private void OnEnable()
    {
        _input.FlashLightPressed += HandleFlashLight;
        _input.BiteFlashLightPressed += HandleBiteFlashLight;
    }
    private void OnDisable()
    {
        _input.FlashLightPressed -= HandleFlashLight;
        _input.BiteFlashLightPressed -= HandleBiteFlashLight;
    }

    private void Awake()
    {
        flashLightCurrentCapacity = FlashLightMaxCapacity;
    }

    private void HandleFlashLight()
    {
        if(flashLightCurrentCapacity > 0)
        {
            if (!isFlashLightEnable)
                FlashLightEnable();
            else
                FlashLightDisable();
        }
    }

    private void HandleBiteFlashLight()
    {
        _playerHealth.TakeDamage(damageToPlayer);
        if (flashLightCurrentCapacity + chargeRecovery > FlashLightMaxCapacity)
        {
            flashLightCurrentCapacity = FlashLightMaxCapacity;
        }
        else
        {
            flashLightCurrentCapacity += chargeRecovery;
        }
        _monsterEars.Ears(transform.position, 3);
        Debug.Log("Откусил фонарик");
    }

    private void FlashLightEnable()
    {
        isFlashLightEnable = true;
        flashLightGameObject.SetActive(true);
        _monsterEars.Ears(transform.position, 1);
    }
    
    private void FlashLightDisable()
    {
        isFlashLightEnable = false;
        flashLightGameObject.SetActive(false);
        _monsterEars.Ears(transform.position, 1);
    }

    private void Update()
    {
        if(!isFlashLightEnable || flashLightCurrentCapacity <= 0)
            return;

        flashLightCurrentCapacity -= rateOfDecrease * Time.deltaTime;
        
        if(flashLightCurrentCapacity <= 0)
            FlashLightDisable();
    }
}
