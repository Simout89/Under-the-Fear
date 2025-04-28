using System;
using UnityEngine;

namespace _Script.Manager
{
    public class GameManager: MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public GameObject Player => player;
    }
}