using System;
using _Script.Utils;
using UnityEngine;
using Zenject;

namespace _Script.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Inject] private MonsterEars _monsterEars;
        [Inject] private GameStateManager _gameStateManager;

        public bool Move => _gameStateManager.PlayerMove;
    
        [Header("References")]
        [SerializeField] public PCKeyInput input;
        [SerializeField] private Transform body;
        [SerializeField] private Transform cameraTarget;

        [Header("Settings")]
        [SerializeField] private float speedMovement = 5;
        public float SpeedMovement => speedMovement;
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private float sneakHeight = 1f;
        [SerializeField] private float sneakSpeed = 1f;
        public float SneakSpeedMovement => sneakSpeed;
        private float sneakSpeedMovement = 0f;
        private float originHeight;

        public event Action OnSprintStarted;
        public event Action OnSprintStopped;
        
        private float _verticalVelocity;
        [HideInInspector] public float _additionalVelocity;
        private CharacterController _characterController;
        public CharacterController CharacterController => _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            originHeight = _characterController.height;
        }

        private void Update()
        {
            if (!Move) return;
            
            Vector2 movement2D = input.GetMovementInput();
            Vector3 movementInput = new Vector3(movement2D.y, VerticalForceCalculator(), -movement2D.x);
        
            float yRotation = cameraTarget.rotation.eulerAngles.y;
            Quaternion characterRotation = Quaternion.Euler(0f, yRotation, 0f);

            if (input.OnSneak())
            {
                _characterController.height = sneakHeight;

                sneakSpeedMovement = sneakSpeed;
                
                OnSprintStopped?.Invoke();
            }
            else
            {
                _characterController.height = originHeight;
                
                sneakSpeedMovement = 0;
                
                if (input.OnSprint())
                {
                    OnSprintStarted?.Invoke();
                }
                else
                {
                    OnSprintStopped?.Invoke();
                }
            }
            Vector3 localMovement = (characterRotation * movementInput);
            _characterController.Move((localMovement * ((speedMovement + _additionalVelocity - sneakSpeedMovement) * Time.deltaTime)));
        }

        private float VerticalForceCalculator()
        {
            if (_characterController.isGrounded)
            {
                _verticalVelocity = -1f;
            }
            else
            {
                _verticalVelocity -= gravity * Time.deltaTime;
            }
            return _verticalVelocity;
        }
    }
}