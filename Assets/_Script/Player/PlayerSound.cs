using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _playerFlashlight;

    [SerializeField] private CharacterController _characterController;

    [Header("Player Settings")]
    [SerializeField] private float footstepInterval = 1f;
    [SerializeField] private float minStepSpeed = 1f;
    
    [Header("Player Events")]
    [SerializeField] private AK.Wwise.Event footStepEvent;

    [Header("FlashLight Events")]
    [SerializeField] private AK.Wwise.Event flashLightEnableEvent;
    [SerializeField] private AK.Wwise.Event flashLightDisableEvent;
    [SerializeField] private AK.Wwise.Event flashLightBiteEvent;

    private TimedInvoker stepSoundInvoker;
    
    private void OnEnable()
    {
        _playerFlashlight.OnFlashLightEnable += HandleFlashLightEnable;
        _playerFlashlight.OnFlashLightDisable += HandleFlashLightDisable;
        _playerFlashlight.OnFlashLightBite += HandleFlashLightBite;
    }
    private void OnDisable()
    {
        _playerFlashlight.OnFlashLightEnable -= HandleFlashLightEnable;
        _playerFlashlight.OnFlashLightDisable -= HandleFlashLightDisable;
        _playerFlashlight.OnFlashLightBite -= HandleFlashLightBite;

    }

    private void HandleFlashLightBite()
    {
        flashLightBiteEvent.Post(gameObject);
    }

    private void HandleFlashLightEnable()
    {
        flashLightEnableEvent.Post(gameObject);
    }
    
    private void HandleFlashLightDisable()
    {
        flashLightDisableEvent.Post(gameObject);
    }

    private void Awake()
    {
        stepSoundInvoker = new TimedInvoker(PlayFootStepSound, footstepInterval);
    }

    private void PlayFootStepSound()
    {
        Debug.Log("Звук Шагов ");
        ChangeSurface();
        footStepEvent.Post(gameObject);
    }

    private void Update()
    {
        var horizontalVelocity = _characterController.velocity;
        horizontalVelocity = new Vector3(horizontalVelocity.x, 0, horizontalVelocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        AkSoundEngine.SetRTPCValue("Footstep_Volume", Mathf.Clamp(horizontalSpeed * 5, 10, 100), gameObject);
        
        Debug.Log(horizontalSpeed);
        
        stepSoundInvoker.SetInterval(Mathf.Lerp(footstepInterval, footstepInterval * 0.8f, (horizontalSpeed - 5) / 5));
        
        if (horizontalSpeed >= minStepSpeed)
        {
            stepSoundInvoker.Tick();
        }
    }

    private void ChangeSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, ~0 ,QueryTriggerInteraction.Ignore))
        {
            if(!hit.collider.CompareTag("Untagged"))
                AkUnitySoundEngine.SetSwitch("Surface", hit.collider.tag, gameObject);
            else
                AkUnitySoundEngine.SetSwitch("Surface", "Concrete", gameObject);
        }
    }
}



public class TimedInvoker
{
    private float interval;
    private float nextInvokeTime;
    private Action action;

    public TimedInvoker(Action action, float interval)
    {
        this.action = action;
        this.interval = interval;
        this.nextInvokeTime = Time.time + interval;
    }

    public void Tick()
    {
        if (Time.time >= nextInvokeTime)
        {
            action?.Invoke();
            nextInvokeTime = Time.time + interval;
        }
    }

    public void ResetTimer()
    {
        nextInvokeTime = Time.time + interval;
    }

    public void SetInterval(float newInterval)
    {
        interval = newInterval;
    }
}