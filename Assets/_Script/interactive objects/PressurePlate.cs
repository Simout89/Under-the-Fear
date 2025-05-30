using System;
using System.Collections;
using _Script.Player;
using UnityEngine;

namespace _Script.interactive_objects
{
    public class PressurePlate: MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event sound;
        [SerializeField] private Material onMaterial;
        [SerializeField] private Material offMaterial;
        public event Action onStay;
        private MeshRenderer _meshRenderer;

        private bool _canTrigger = true;

        private void Awake()
        {
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_canTrigger && other.GetComponent<PlayerController>() != null)
            {
                onStay?.Invoke();
            }
        }

        public void Restart()
        {
            _canTrigger = true;
            _meshRenderer.material = offMaterial;
        }

        public void Right()
        {
            _meshRenderer.material = onMaterial;
            _canTrigger = false;
            sound.Post(gameObject);
        }

    }
}