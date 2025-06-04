using System;
using UnityEngine;
using Zenject;

namespace _Script.interactive_objects
{
    public class SaveZone: MonoBehaviour
    {
        [Inject] private SaveManager _saveManager;
        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CharacterController>())
            {
                _saveManager.Save();
            }
        }
    }
}