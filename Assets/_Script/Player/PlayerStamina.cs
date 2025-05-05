using System;
using _Script.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Script.Player
{
    public class PlayerStamina: MonoBehaviour
    {
        [Inject] private MonsterEars _monsterEars;
        [SerializeField] private PlayerController _playerController;
        
        private EnduranceSystem sprintStamina;

        [Header("Settings")]
        [SerializeField] private float maxValue = 50;
        [SerializeField] private float minValue = 0;
        [SerializeField] private float rateOfDecrease = 5;
        [SerializeField] private float rateOfIncrease = 5;
        [SerializeField] private float sprintSpeed = 3;
        [SerializeField] private int soundStrength = 4;
        
        [BoxGroup("StaminaCurrentCapacity")]
        [HideLabel]
        [ProgressBar("Min", "Max",r: 0, g: 0, b: 255, Height = 30)]
        [ReadOnly][SerializeField] private float CurrentCapacity;
    
        [BoxGroup("CurrentHealthPoint")]
        private float Min => minValue;

        [BoxGroup("CurrentHealthPoint")]
        private float Max => maxValue;

        private void OnEnable()
        {
            _playerController.OnSprintStarted += HandleSprintStarted;
            _playerController.OnSprintStopped += HandleSprintStopped;
        }

        private void OnDisable()
        {
            _playerController.OnSprintStarted -= HandleSprintStarted;
            _playerController.OnSprintStopped -= HandleSprintStopped;
        }

        private void HandleSprintStarted()
        {
            sprintStamina.ReduceEndurance();

            if (sprintStamina.CurrentEndurance <= 0)
            {
                _playerController._additionalVelocity = 0;
            }
            else
            {
                _playerController._additionalVelocity = sprintSpeed;
                
                _monsterEars.Ears(transform.position, soundStrength);
            }

            CurrentCapacity = sprintStamina.CurrentEndurance;
        }

        private void HandleSprintStopped()
        {
            _playerController._additionalVelocity = 0;
            
            sprintStamina.AddEndurance();
            
            CurrentCapacity = sprintStamina.CurrentEndurance;
        }

        private void Awake()
        {
            sprintStamina = new EnduranceSystem(maxValue, minValue, rateOfDecrease, rateOfIncrease);
        }
    }
}