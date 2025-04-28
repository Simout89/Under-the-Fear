using System;
using UnityEngine;
using Zenject;

namespace _Script.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Inject] private MonsterEars monsterEars;
        [Inject] private GameStateManager _gameStateManager;

        public bool Move => _gameStateManager.PlayerMove;
    
        [Header("References")]
        [SerializeField] public PCKeyInput input;
        [SerializeField] private Transform body;
        [SerializeField] private Transform cameraTarget;

        [Header("Settings")]
        [SerializeField] private float speedMovement = 5;
        [SerializeField] private float sprintSpeedMovement = 5;
        [SerializeField] private float gravity = 9.8f;
        
        private float _verticalVelocity;
        private float _additionalVelocity;
        private CharacterController _characterController;
        public CharacterController CharacterController => _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!Move) return;
            
            Vector2 movement2D = input.GetMovementInput();
            Vector3 movementInput = new Vector3(movement2D.y, VerticalForceCalculator(), -movement2D.x);
        
            float yRotation = cameraTarget.rotation.eulerAngles.y;
            Quaternion characterRotation = Quaternion.Euler(0f, yRotation, 0f);

            if (input.OnSprint()) // TODO: доделать
            {
                _additionalVelocity = sprintSpeedMovement;
                monsterEars.Ears(transform.position, 4);
            }
            else
                _additionalVelocity = 0f;

            Vector3 localMovement = (characterRotation * movementInput);
            _characterController.Move((localMovement * ((speedMovement + _additionalVelocity) * Time.deltaTime)));
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