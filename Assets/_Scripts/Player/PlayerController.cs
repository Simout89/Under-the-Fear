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
        
        [Header("Crouch Settings")]
        [SerializeField] private LayerMask obstacleLayers = -1; // Слои препятствий
        [SerializeField] private float headCheckOffset = 0.1f; // Отступ для проверки над головой
        [SerializeField] private float standUpCheckRadius = 0.3f; // Радиус проверки при вставании
        
        private float sneakSpeedMovement = 0f;
        private float originHeight;
        private bool isCrouching = false;
        private bool wantsToStand = false; // Флаг желания встать

        public event Action OnSprintStarted;
        public event Action OnSprintStopped;
        public event Action OnCrouchStarted;
        public event Action OnCrouchStopped;
        
        private float _verticalVelocity;
        [HideInInspector] public float _additionalVelocity;
        private CharacterController _characterController;
        public CharacterController CharacterController => _characterController;

        // Публичные свойства для получения состояния
        public bool IsCrouching => isCrouching;
        public bool CanStandUp => !IsObstacleAboveHead();

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

            HandleCrouchLogic();

            Vector3 localMovement = (characterRotation * movementInput);
            _characterController.Move((localMovement * ((speedMovement + _additionalVelocity - sneakSpeedMovement) * Time.deltaTime)));
        }

        private void HandleCrouchLogic()
        {
            bool sneakInput = input.OnSneak();

            if (sneakInput && !isCrouching)
            {
                // Начинаем приседание
                StartCrouch();
            }
            else if (!sneakInput && isCrouching)
            {
                // Хотим встать
                wantsToStand = true;
                
                if (CanStandUp)
                {
                    // Можем встать - препятствий нет
                    StopCrouch();
                }
                else
                {
                    // Не можем встать - есть препятствие над головой
                    // Остаемся в присяде
                }
            }
            else if (sneakInput && isCrouching)
            {
                // Продолжаем сидеть по желанию игрока
                wantsToStand = false;
            }
            else if (!sneakInput && !isCrouching)
            {
                // Стоим нормально
                wantsToStand = false;
                
                // Обрабатываем спринт только когда не сидим
                if (input.OnSprint())
                {
                    OnSprintStarted?.Invoke();
                }
                else
                {
                    OnSprintStopped?.Invoke();
                }
            }

            // Проверяем можем ли встать если хотели встать
            if (wantsToStand && isCrouching && CanStandUp)
            {
                StopCrouch();
            }
        }

        private void StartCrouch()
        {
            if (isCrouching) return;

            isCrouching = true;
            wantsToStand = false;
            
            _characterController.height = sneakHeight;
            sneakSpeedMovement = sneakSpeed;
            
            OnSprintStopped?.Invoke();
            OnCrouchStarted?.Invoke();
            
        }

        private void StopCrouch()
        {
            if (!isCrouching) return;

            isCrouching = false;
            wantsToStand = false;
            
            _characterController.height = originHeight;
            sneakSpeedMovement = 0;
            
            OnCrouchStopped?.Invoke();
            
        }

        /// <summary>
        /// Проверяет есть ли препятствие над головой игрока
        /// </summary>
        private bool IsObstacleAboveHead()
        {
            Vector3 characterCenter = transform.position + _characterController.center;
            
            // Позиция для проверки (верх персонажа при полном росте)
            float checkHeight = (originHeight - sneakHeight) + headCheckOffset;
            Vector3 checkPosition = characterCenter + Vector3.up * (sneakHeight / 2 + checkHeight / 2);
            
            // Проверяем капсулой (как Character Controller) - игнорируем триггеры
            bool hasObstacle = Physics.CheckCapsule(
                checkPosition + Vector3.up * (checkHeight / 2 - standUpCheckRadius),
                checkPosition - Vector3.up * (checkHeight / 2 - standUpCheckRadius),
                standUpCheckRadius,
                obstacleLayers,
                QueryTriggerInteraction.Ignore // Игнорируем триггеры
            );

            // Дополнительная проверка простым raycast вверх - игнорируем триггеры
            bool hasObstacleRay = Physics.Raycast(
                characterCenter + Vector3.up * (sneakHeight / 2),
                Vector3.up,
                checkHeight + headCheckOffset,
                obstacleLayers,
                QueryTriggerInteraction.Ignore // Игнорируем триггеры
            );

            return hasObstacle || hasObstacleRay;
        }

        /// <summary>
        /// Принудительно встать (если возможно)
        /// </summary>
        public void ForceStandUp()
        {
            if (isCrouching && CanStandUp)
            {
                StopCrouch();
            }
        }

        /// <summary>
        /// Принудительно присесть
        /// </summary>
        public void ForceCrouch()
        {
            if (!isCrouching)
            {
                StartCrouch();
            }
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

        // Метод для отладки - показывает зону проверки препятствий
        private void OnDrawGizmosSelected()
        {
            if (_characterController == null) return;

            Vector3 characterCenter = transform.position + _characterController.center;
            
            // Показываем зону проверки препятствий
            if (isCrouching)
            {
                float checkHeight = (originHeight - sneakHeight) + headCheckOffset;
                Vector3 checkPosition = characterCenter + Vector3.up * (sneakHeight / 2 + checkHeight / 2);
                
                Gizmos.color = IsObstacleAboveHead() ? Color.red : Color.green;
                
                // Рисуем капсулу проверки
                Gizmos.DrawWireSphere(checkPosition + Vector3.up * (checkHeight / 2 - standUpCheckRadius), standUpCheckRadius);
                Gizmos.DrawWireSphere(checkPosition - Vector3.up * (checkHeight / 2 - standUpCheckRadius), standUpCheckRadius);
                
                // Рисуем линию raycast
                Gizmos.color = Color.yellow;
                Vector3 rayStart = characterCenter + Vector3.up * (sneakHeight / 2);
                Vector3 rayEnd = rayStart + Vector3.up * (checkHeight + headCheckOffset);
                Gizmos.DrawLine(rayStart, rayEnd);
            }
            
            // Показываем текущий Character Controller
            Gizmos.color = Color.blue;
            Vector3 bottom = characterCenter - Vector3.up * (_characterController.height / 2 - _characterController.radius);
            Vector3 top = characterCenter + Vector3.up * (_characterController.height / 2 - _characterController.radius);
            Gizmos.DrawWireSphere(bottom, _characterController.radius);
            Gizmos.DrawWireSphere(top, _characterController.radius);
        }
    }
}