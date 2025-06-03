using System;
using _Script.Puzzle;
using UnityEngine;
using Zenject;

namespace _Script.Manager
{
    public class GameManager: MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private int firstPuzzleId;
        [SerializeField] private bool spawnOnStart = false;
        [Inject] private MonsterAi _monsterAi;
        [Inject] private PuzzleManager _puzzleManager;
        public GameObject Player => player;

        private void Awake()
        {
            if(!spawnOnStart)
                _monsterAi.Disable();
        }

        private void OnEnable()
        {
            _puzzleManager.OnPuzzleSolved += HandlePuzzleSolved;
        }

        private void OnDisable()
        {
            _puzzleManager.OnPuzzleSolved -= HandlePuzzleSolved;
        }

        private void HandlePuzzleSolved(int obj)
        {
            if (firstPuzzleId == obj)
            {
                _monsterAi.Enable();
            }
        }
    }
}