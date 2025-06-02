using System;
using _Script.Player;
using _Script.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Zenject;

public class PlayerFlashlight : SerializedMonoBehaviour
{
    public event Action OnFlashLightEnable;
    public event Action OnFlashLightDisable;
    public event Action OnFlashLightBite;
    
    
    [Inject] private MonsterEars _monsterEars;
    
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject flashLightGameObject;
    [OdinSerialize] private PlayerHealth _playerHealth;
    [SerializeField] private EnduranceSlider _enduranceSlider;
    private EnduranceSystem _enduranceSystem;

    [Header("Settings")]
    [SerializeField] private float FlashLightMaxCapacity = 50;
    
    [BoxGroup("FlashLightCurrentCapacity")]
    [HideLabel]
    [ProgressBar("Min", "Max",r: 255, g: 255, b: 0, Height = 30)]
    [ReadOnly][SerializeField] private float flashLightCurrentCapacity;
    
    [BoxGroup("CurrentHealthPoint")]
    private float Min = 0;

    [BoxGroup("CurrentHealthPoint")]
    private float Max => FlashLightMaxCapacity;
    
    
    [SerializeField] private float rateOfDecrease = 1;
    [SerializeField] private float damageToPlayer = 10;
    [SerializeField] private float chargeRecovery = 20;

    private IInput _input => _playerController.input;

    public bool isFlashLightEnable { get; private set; }

    private void OnEnable()
    {
        _input.FlashLightPressed += HandleFlashLight;
        _input.BiteFlashLightPressed += HandleBiteFlashLight;
        _playerHealth.onPlayerDeath += HandlePlayerDeath;
    }
    private void OnDisable()
    {
        _input.FlashLightPressed -= HandleFlashLight;
        _input.BiteFlashLightPressed -= HandleBiteFlashLight;
        _playerHealth.onPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        _enduranceSystem.SetValue(FlashLightMaxCapacity);
    }

    private void Awake()
    {
        flashLightCurrentCapacity = FlashLightMaxCapacity;
        _enduranceSystem = new EnduranceSystem(FlashLightMaxCapacity, 0, flashLightCurrentCapacity);
        _enduranceSlider.Initialization(_enduranceSystem);
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
            _enduranceSystem.SetValue(flashLightCurrentCapacity);
        }
        else
        {
            flashLightCurrentCapacity += chargeRecovery;
            _enduranceSystem.SetValue(flashLightCurrentCapacity);
        }
        _monsterEars.Ears(transform.position, 3);
        Debug.Log("Откусил фонарик");
        OnFlashLightBite?.Invoke();
    }

    private void FlashLightEnable()
    {
        isFlashLightEnable = true;
        flashLightGameObject.SetActive(true);
        _monsterEars.Ears(transform.position, 1);
        OnFlashLightEnable?.Invoke();
    }
    
    private void FlashLightDisable()
    {
        isFlashLightEnable = false;
        flashLightGameObject.SetActive(false);
        _monsterEars.Ears(transform.position, 1);
        OnFlashLightDisable?.Invoke();
    }

    private void Update()
    {
        if(!isFlashLightEnable || flashLightCurrentCapacity <= 0)
            return;

        flashLightCurrentCapacity -= rateOfDecrease * Time.deltaTime;
        _enduranceSystem.SetValue(flashLightCurrentCapacity);

        
        if(flashLightCurrentCapacity <= 0)
            FlashLightDisable();
    }
}
