using System;
using _Script.Player;
using UnityEngine;
using Zenject;

public class PlayerInteractor : MonoBehaviour
{
    [Inject] private UiManager _uiManager;
    
    [Header("References")] [SerializeField]
    private FirstPersonCamera firstPersonCamera;

    [SerializeField] private PlayerController _playerController;
    private IInput _input => _playerController.input;
    
    [Header("Settings")] [SerializeField] private float maxDistance;

    private IClickable clickable;
    private Camera _camera;

    private void Awake()
    {
        _camera = firstPersonCamera.CameraPlayer.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _input.InteractPressed += HandleInteractPressed;
    }

    private void OnDisable()
    {
        _input.InteractPressed -= HandleInteractPressed;
    }

    private void HandleInteractPressed()
    {
        if (clickable != null)
        {
            clickable.Click();
        }
    }

    private void FixedUpdate()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~6, QueryTriggerInteraction.Ignore )) {
            if (hit.collider.TryGetComponent<IClickable>(out IClickable clickable))
            {
                this.clickable = clickable;
                _uiManager.ShowPoint();
            }
            else
            {
                this.clickable = null;
                _uiManager.HidePoint();
            }
        }
        else
        {
            this.clickable = null;
            _uiManager.HidePoint();
        }
    }
}
