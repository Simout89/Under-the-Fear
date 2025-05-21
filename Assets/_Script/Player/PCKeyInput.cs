using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Input/Player Input")]
public class PCKeyInput : ScriptableObject, IInput
{
    [SerializeField] private InputActionAsset _inputActionAsset;
    public event Action FlashLightPressed;
    public event Action BiteFlashLightPressed;
    public event Action InteractPressed;
    public event Action InteractReleased;
    public event Action ThrowPressed;

    private void OnEnable()
    {
        _inputActionAsset.FindAction("FlashLight").performed += OnFlashLightActionPerformed;
        _inputActionAsset.FindAction("BiteOffFlashLight").performed += OnBiteOffFlashLightPerformed;
        _inputActionAsset.FindAction("Interact").performed += OnInteractPressedPerformed;
        _inputActionAsset.FindAction("Interact").canceled += OnInteractPressedReleased;
        _inputActionAsset.FindAction("Throw").performed += OnThrowPressedPerformed;
    }
    private void OnDisable()
    {
        _inputActionAsset.FindAction("FlashLight").performed -= OnFlashLightActionPerformed;
        _inputActionAsset.FindAction("BiteOffFlashLight").performed -= OnBiteOffFlashLightPerformed;
        _inputActionAsset.FindAction("Interact").performed -= OnInteractPressedPerformed;
        _inputActionAsset.FindAction("Interact").canceled -= OnInteractPressedReleased;
        _inputActionAsset.FindAction("Throw").performed -= OnThrowPressedPerformed;
    }

    private void OnInteractPressedReleased(InputAction.CallbackContext obj)
    {
        InteractReleased?.Invoke();
    }

    private void OnThrowPressedPerformed(InputAction.CallbackContext obj)
    {
        ThrowPressed?.Invoke();
    }

    public bool OnSneak()
    {
        return _inputActionAsset.FindAction("Sneak").IsPressed();
    }

    private void OnInteractPressedPerformed(InputAction.CallbackContext obj)
    {
        InteractPressed?.Invoke();
    }

    public Vector2 GetMovementInput()
    {
        return _inputActionAsset.FindAction("Move").ReadValue<Vector2>();
    }
    
    private void OnBiteOffFlashLightPerformed(InputAction.CallbackContext obj)
    {
        BiteFlashLightPressed?.Invoke();
    }
    
    private void OnFlashLightActionPerformed(InputAction.CallbackContext context)
    {
        FlashLightPressed?.Invoke();
    }

    public Vector2 GetLookInput()
    {
        return _inputActionAsset.FindAction("Look").ReadValue<Vector2>();
    }

    public bool OnJump()
    {
        return _inputActionAsset.FindAction("Jump").IsPressed();
    }
    
    public bool OnSprint()
    {
        return _inputActionAsset.FindAction("Sprint").IsPressed();
    }
}

public interface IInput
{
    public Vector2 GetMovementInput();

    public Vector2 GetLookInput();
    public bool OnJump();
    public bool OnSprint();
    public bool OnSneak();
    public event Action FlashLightPressed;
    public event Action BiteFlashLightPressed;
    public event Action InteractPressed;
    public event Action InteractReleased;
    public event Action ThrowPressed;
}