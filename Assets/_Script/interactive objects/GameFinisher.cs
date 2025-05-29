using System;
using _Script.Puzzle;
using UnityEngine;
using Zenject;

namespace _Script.interactive_objects
{
    public class GameFinisher: MonoBehaviour
    {
        [Inject] private PuzzleManager _puzzleManager;
        [SerializeField] private ButtonTrigger _trigger;

        private void OnEnable()
        {
            _trigger.onClick += HandleClick;
        }

        private void OnDisable()
        {
            _trigger.onClick -= HandleClick;
        }

        private void HandleClick()
        {
            if (_puzzleManager.PuzzlesSolved)
            {
                Debug.Log("Игра продейна");
            }
        }
    }
}