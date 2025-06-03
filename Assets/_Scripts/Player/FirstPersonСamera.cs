using System;
using _Script.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Script.Player
{
    public class FirstPersonCamera: MonoBehaviour
    {
    
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        private IInput input => playerController.input;
    
        private bool Move => playerController.Move;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform cameraPlayer;
        public Transform CameraPlayer => cameraPlayer;

        [Inject] private PlayerSettings _settingsManager;
        private float Sensitivity => _settingsManager.SettingsConfig.MouseSensitivity;

        [Header("Settings")]
        [SerializeField] private float _sensitivityMultiplayer = 1f;

        private float _xRotation;
        private float _yRotation;

        private void Awake()
        {
            if (cameraPlayer == null)
            {
                cameraPlayer = Camera.main.transform;
            }

            _xRotation = 0;
            _yRotation = 0;
        }

        public void LateUpdate()
        {
            _xRotation += input.GetLookInput().y * Time.deltaTime * Sensitivity * _sensitivityMultiplayer;
            _xRotation = Mathf.Clamp(_xRotation, -89.9f, 89.9f);
        
            _yRotation += input.GetLookInput().x * Time.deltaTime * Sensitivity * _sensitivityMultiplayer;
        
            cameraPivot.localRotation = Quaternion.Euler(0, _yRotation, _xRotation);
        }
    }
}
