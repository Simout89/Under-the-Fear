using System;
using _Script.Player;
using UnityEngine;
using Zenject;

public class PlayerInteractor : MonoBehaviour
{
    [Inject] private UiManager _uiManager;
    
    [Header("References")] [SerializeField]
    private FirstPersonCamera firstPersonCamera;
    [SerializeField] private Transform holdPoint;

    [SerializeField] private PlayerController _playerController;
    private IInput _input => _playerController.input;
    
    [Header("Settings")] [SerializeField] private float maxDistance;

    private IClickable clickable;
    private GameObject clickableGameObject; 
    private IStorable storable;
    private GameObject pickupable;
    private GameObject holdGameObject;
    private Camera _camera;

    private void Awake()
    {
        _camera = firstPersonCamera.CameraPlayer.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _input.InteractPressed += HandleInteractPressed;
        _input.ThrowPressed += HandleThrowPressed;
        _input.InteractReleased += HandleReleased;
    }

    private void OnDisable()
    {
        _input.InteractPressed -= HandleInteractPressed;
        _input.ThrowPressed -= HandleThrowPressed;
        _input.InteractReleased -= HandleReleased;

    }

    private void HandleReleased()
    {
        if (clickableGameObject != null)
        {
            if (clickableGameObject.TryGetComponent(out IReleasable releasable))
            {
                releasable.OnRelease();
            }
        }
    }

    private void HandleThrowPressed()
    {
        if (holdGameObject != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(_playerController.transform.position, Vector3.down, out hit, 10, ~6, QueryTriggerInteraction.Ignore))
            {
                holdGameObject.transform.parent = null;
                var box = holdGameObject.GetComponent<BoxCollider>();
                var heightOffset = Vector3.Scale(box.size, holdGameObject.transform.lossyScale).y / 2f;
                holdGameObject.transform.position = hit.point + new Vector3(0, heightOffset, 0);
            }
            
            holdGameObject.GetComponent<IPickupable>().PlayThrowSound();
        
            holdGameObject = null;
        }
    }

    private void HandleInteractPressed()
    {
        if (clickable != null)
        {
            clickable.Click();
        }

        if (pickupable != null)
        {
            pickupable.transform.parent = holdPoint.transform;
            pickupable.transform.localPosition = Vector3.zero;

            holdGameObject = pickupable;
            
            pickupable.GetComponent<IPickupable>().PlayPickupSound();
        }
        
        if (storable != null)
        {
            if (holdGameObject != null && storable.HoldingItem == null)
            {
                holdGameObject.transform.parent = storable.HoldingPoint.transform;
                holdGameObject.transform.localPosition = new Vector3();
                
                holdGameObject.GetComponent<IPickupable>().PlayPickupSound();

                if (holdGameObject.TryGetComponent(out Collider collider))
                {
                    collider.enabled = false;
                }
            
                storable.HoldingItem = holdGameObject;
                
                
                holdGameObject = null;
            }else if (holdGameObject == null && storable.HoldingItem != null)
            {
                holdGameObject = storable.HoldingItem;
                
                holdGameObject.GetComponent<IPickupable>().PlayPickupSound();
                
                holdGameObject.transform.parent = holdPoint.transform;
                holdGameObject.transform.localPosition = Vector3.zero;
                
                if (holdGameObject.TryGetComponent(out Collider collider))
                {
                    collider.enabled = true;
                }

                storable.HoldingItem = null;
            }
        }
    }

    private void FixedUpdate()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~6, QueryTriggerInteraction.Ignore )) {
            if (hit.collider.TryGetComponent(out IClickable clickable))
            {
                clickableGameObject = hit.collider.gameObject;
                this.clickable = clickable;
                _uiManager.ShowPoint();
            }
            else
            {
                if (hit.collider.TryGetComponent(out IReleasable releasable))
                {
                    releasable.OnRelease();
                }
                this.clickable = null;
                clickableGameObject = null;
                _uiManager.HidePoint();
            }

            if (hit.collider.TryGetComponent(out IPickupable pickupable))
            {
                this.pickupable = hit.collider.gameObject;
            }
            else
            {
                this.pickupable = null;
            }
            
            if (hit.collider.TryGetComponent(out IStorable storable))
            {
                this.storable = storable;
            }
            else
            {
                this.storable = null;
            }
        }
        else
        {
            if (clickableGameObject&& clickableGameObject.TryGetComponent(out IReleasable releasable))
            {
                releasable.OnRelease();
            }
            this.clickable = null;
            this.pickupable = null;
            this.storable = null;
            clickableGameObject = null;
            _uiManager.HidePoint();
        }
    }
}
