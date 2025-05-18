using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Script.Puzzle
{
    public class PuzzleValidator : PuzzleBase
    {
        [OdinSerialize] private ISwitchable[] _switchables;
        [SerializeField] private bool[] trueState;

        private void Awake()
        {
            ChangeTrueState();
        }

        private void ChangeTrueState()
        {
            trueState = new bool[_switchables.Length];

            for (int i = 0; i < trueState.Length; i++)
            {
                trueState[i] = Random.Range(0, 2) == 0;
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < _switchables.Length; i++)
            {
                _switchables[i].OnSwitchState += HandleSwitchState;
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < _switchables.Length; i++)
            {
                _switchables[i].OnSwitchState -= HandleSwitchState;
            }
        }
        private void HandleSwitchState(bool obj)
        {
            for (int i = 0; i < _switchables.Length; i++)
            {
                if(_switchables[i].Status != trueState[i])
                {
                    IsSolved = false;
                    return;
                }
            }
            Debug.Log("Головоломка решена");

            IsSolved = true;
            
            OnPuzzleSolved();
        }
    }
}
