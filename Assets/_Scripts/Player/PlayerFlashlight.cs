using System;
using System.Collections;
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
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private EnduranceSlider _enduranceSlider;
    private EnduranceSystem _enduranceSystem;

    [Header("Settings")]
    [SerializeField] private float FlashLightMaxCapacity = 50;
    
    [BoxGroup("FlashLightCurrentCapacity")]
    [HideLabel]
    [ProgressBar("Min", "Max", r: 255, g: 255, b: 0, Height = 30)]
    [ReadOnly][SerializeField] private float flashLightCurrentCapacity;
    
    [BoxGroup("CurrentHealthPoint")]
    private float Min = 0;

    [BoxGroup("CurrentHealthPoint")]
    private float Max => FlashLightMaxCapacity;
    
    [SerializeField] private float rateOfDecrease = 1;
    [SerializeField] private float damageToPlayer = 10;
    [SerializeField] private float chargeRecovery = 20;

    [Header("Anti-Spam Settings")]
    [SerializeField] private float flashlightToggleCooldown = 0.2f; // Кулдаун между включением/выключением
    [SerializeField] private float biteCooldown = 2f; // Кулдаун между "укусами"
    [SerializeField] private float minFlashlightActiveDuration = 0.1f; // Минимальное время активности фонарика

    // Переменные для контроля задержек
    private float _lastToggleTime = -1f;
    private float _lastBiteTime = -1f;
    private float _flashlightEnabledTime = -1f;
    private bool _canToggle = true;
    private bool _canBite = true;

    private IInput _input => _playerController.input;

    public bool isFlashLightEnable { get; private set; }
    public bool CanToggleFlashlight => _canToggle && flashLightCurrentCapacity > 0;
    public bool CanBiteFlashlight => _canBite;

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
        flashLightCurrentCapacity = FlashLightMaxCapacity;
        _enduranceSystem.SetValue(flashLightCurrentCapacity);
        // Сбрасываем все кулдауны при смерти игрока
        _canToggle = true;
        _canBite = true;
        _lastToggleTime = -1f;
        _lastBiteTime = -1f;
    }

    private void Awake()
    {
        flashLightCurrentCapacity = FlashLightMaxCapacity;
        _enduranceSystem = new EnduranceSystem(FlashLightMaxCapacity, 0, flashLightCurrentCapacity);
        _enduranceSlider.Initialization(_enduranceSystem);
    }

    private void HandleFlashLight()
    {
        if (!CanToggleFlashlight)
        {
            Debug.Log($"Фонарик на кулдауне. Осталось: {GetToggleCooldownRemaining():F1}с");
            return;
        }

        // Проверяем минимальное время активности при выключении
        if (isFlashLightEnable && Time.time - _flashlightEnabledTime < minFlashlightActiveDuration)
        {
            Debug.Log("Слишком быстро! Подождите немного перед выключением фонарика");
            return;
        }

        if (!isFlashLightEnable)
            FlashLightEnable();
        else
            FlashLightDisable();

        StartToggleCooldown();
    }

    private void HandleBiteFlashLight()
    {
        if (!CanBiteFlashlight)
        {
            Debug.Log($"Укус на кулдауне. Осталось: {GetBiteCooldownRemaining():F1}с");
            return;
        }

        _playerHealth.TakeDamage(damageToPlayer);
        
        if (flashLightCurrentCapacity + chargeRecovery > FlashLightMaxCapacity)
        {
            flashLightCurrentCapacity = FlashLightMaxCapacity;
        }
        else
        {
            flashLightCurrentCapacity += chargeRecovery;
        }
        
        _enduranceSystem.SetValue(flashLightCurrentCapacity);
        _monsterEars.Ears(transform.position, 3);
        
        Debug.Log("Откусил фонарик");
        OnFlashLightBite?.Invoke();
        
        StartBiteCooldown();
    }

    private void FlashLightEnable()
    {
        isFlashLightEnable = true;
        _flashlightEnabledTime = Time.time;
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

    private void StartToggleCooldown()
    {
        _lastToggleTime = Time.time;
        _canToggle = false;
        StartCoroutine(ToggleCooldownCoroutine());
    }

    private void StartBiteCooldown()
    {
        _lastBiteTime = Time.time;
        _canBite = false;
        StartCoroutine(BiteCooldownCoroutine());
    }

    private IEnumerator ToggleCooldownCoroutine()
    {
        yield return new WaitForSeconds(flashlightToggleCooldown);
        _canToggle = true;
    }

    private IEnumerator BiteCooldownCoroutine()
    {
        yield return new WaitForSeconds(biteCooldown);
        _canBite = true;
    }

    private float GetToggleCooldownRemaining()
    {
        if (_canToggle) return 0f;
        return Mathf.Max(0f, flashlightToggleCooldown - (Time.time - _lastToggleTime));
    }

    private float GetBiteCooldownRemaining()
    {
        if (_canBite) return 0f;
        return Mathf.Max(0f, biteCooldown - (Time.time - _lastBiteTime));
    }

    private void Update()
    {
        if (!isFlashLightEnable || flashLightCurrentCapacity <= 0)
            return;

        flashLightCurrentCapacity -= rateOfDecrease * Time.deltaTime;
        _enduranceSystem.SetValue(flashLightCurrentCapacity);

        if (flashLightCurrentCapacity <= 0)
            FlashLightDisable();
    }
}