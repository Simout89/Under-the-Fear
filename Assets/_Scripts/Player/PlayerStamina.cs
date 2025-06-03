using System;
using _Script.Puzzle;
using _Script.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Script.Player
{
    public class PlayerStamina: MonoBehaviour
    {
        [Inject] private MonsterEars _monsterEars;
        [Inject] private PlayerHealth _playerHealth;
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private EnduranceSlider staminaSlider;
        
        private EnduranceSystem sprintStamina;
        private EnduranceSystem auditoryAlert;

        [Header("Settings")]
        [SerializeField] private float maxValue = 50;
        [SerializeField] private float minValue = 0;
        [SerializeField] private float rateOfDecrease = 5;
        [SerializeField] private float rateOfIncrease = 5;
        [SerializeField] private float sprintSpeed = 3;
        [Header("Settings Auditory Alert")]
        [SerializeField] private float maxValueAuditoryAlert = 50;
        [SerializeField] private float minValueAuditoryAlert = 0;
        [SerializeField] private float rateOfDecreaseAuditoryAlert = 5;
        [SerializeField] private float rateOfIncreaseAuditoryAlert = 5;
        [SerializeField] private int soundStrength = 4;
        
        [BoxGroup("StaminaCurrentCapacity")]
        [HideLabel]
        [ProgressBar("Min", "Max",r: 0, g: 0, b: 255, Height = 30)]
        [ReadOnly][SerializeField] private float CurrentCapacity;
    
        [BoxGroup("CurrentHealthPoint")]
        private float Min => minValue;

        [BoxGroup("CurrentHealthPoint")]
        private float Max => maxValue;
        
        [BoxGroup("AuditoryAlertCapacity")]
        [HideLabel]
        [ProgressBar("MinAuditoryAlert", "MamAuditoryAlert",r: 139, g: 0, b: 255, Height = 15)]
        [ReadOnly][SerializeField] private float CurrentCapacityAuditoryAlert;
    
        [BoxGroup("CurrentHealthPoint")]
        private float MinAuditoryAlert => minValueAuditoryAlert;

        [BoxGroup("CurrentHealthPoint")]
        private float MamAuditoryAlert => maxValueAuditoryAlert;

        private void OnEnable()
        {
            _playerController.OnSprintStarted += HandleSprintStarted;
            _playerController.OnSprintStopped += HandleSprintStopped;
            _playerHealth.onPlayerDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            _playerController.OnSprintStarted -= HandleSprintStarted;
            _playerController.OnSprintStopped -= HandleSprintStopped;
            _playerHealth.onPlayerDeath -= HandlePlayerDeath;
        }

        private void HandlePlayerDeath()
        {
            sprintStamina.SetValue(maxValue);
        }

        private void HandleSprintStarted()
        {
            if(_playerController.input.GetMovementInput() == Vector2.zero)
            {
                HandleSprintStopped();
                return;
            }
            
            sprintStamina.ReduceEndurance();
            
            auditoryAlert.AddEndurance();

            if (sprintStamina.CurrentEndurance <= 0)
            {
                _playerController._additionalVelocity = 0;
            }
            else
            {
                _playerController._additionalVelocity = sprintSpeed;
            }

            if (auditoryAlert.CurrentEndurance >= maxValueAuditoryAlert)
            {
                _monsterEars.Ears(transform.position, soundStrength);
            }

            CurrentCapacity = sprintStamina.CurrentEndurance;

            CurrentCapacityAuditoryAlert = auditoryAlert.CurrentEndurance;
        }

        private void HandleSprintStopped()
        {
            _playerController._additionalVelocity = 0;
            
            sprintStamina.AddEndurance();
            
            auditoryAlert.ReduceEndurance();
            
            CurrentCapacity = sprintStamina.CurrentEndurance;
            
            CurrentCapacityAuditoryAlert = auditoryAlert.CurrentEndurance;
        }

        private void Awake()
        {
            sprintStamina = new EnduranceSystem(maxValue, minValue, rateOfDecrease, rateOfIncrease);
            auditoryAlert = new EnduranceSystem(maxValueAuditoryAlert, minValueAuditoryAlert, rateOfDecreaseAuditoryAlert, rateOfIncreaseAuditoryAlert, 0);
            staminaSlider.Initialization(sprintStamina);
        }
    }
}