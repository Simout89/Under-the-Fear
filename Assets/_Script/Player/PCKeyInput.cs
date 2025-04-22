using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Input/Player Input")]
public class PCKeyInput : ScriptableObject, IInput
{
    [SerializeField] private InputActionAsset _inputActionAsset;
    
    public Vector2 GetMovementInput()
    {
        return _inputActionAsset.FindAction("Move").ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return _inputActionAsset.FindAction("Look").ReadValue<Vector2>();
    }

    public bool OnJump()
    {
        return _inputActionAsset.FindAction("Jump").IsPressed();
    }
}

public interface IInput
{
    public Vector2 GetMovementInput();

    public Vector2 GetLookInput();
    public bool OnJump();
}