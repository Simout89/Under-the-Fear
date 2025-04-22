using System;
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
        
        // private float Sensitivity => _settingsManager.sensitivity;

        private float _xRotation;
        private float _yRotation;

        private void Awake()
        {
            if (cameraPlayer == null)
            {
                cameraPlayer = Camera.main.transform;
            }
        }

        public void LateUpdate()
        {
            _xRotation += input.GetLookInput().y * Time.deltaTime * 10;
            _xRotation = Mathf.Clamp(_xRotation, -89.9f, 89.9f);
        
            _yRotation += input.GetLookInput().x * Time.deltaTime * 10;
        
            cameraPivot.localRotation = Quaternion.Euler(0, _yRotation, _xRotation);
        }
    }
}
