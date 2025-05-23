using System;
using UnityEngine;
using Random = System.Random;

namespace _Script.Puzzle.Solution
{
    public class LeverPuzzleSolution: PuzzleSolutionBase
    {
        [SerializeField] private LeverPuzzleHintUnit[] _leverPuzzleHintUnit;
        public override void Restart(bool[] answer)
        {
            Random rnd = new Random();
            for (int i = 0; i < answer.Length; i++)
            {
                if (rnd.Next(0, 2) == 0)
                {
                    _leverPuzzleHintUnit[i].Lamp.SetActive(true);
                    if (answer[i])
                    {
                        _leverPuzzleHintUnit[i].LiarSign.SetActive(false);
                    }
                    else
                    {
                        _leverPuzzleHintUnit[i].LiarSign.SetActive(true);
                    }
                }
                else
                {
                    _leverPuzzleHintUnit[i].Lamp.SetActive(false);
                    if (answer[i])
                    {
                        _leverPuzzleHintUnit[i].LiarSign.SetActive(true);
                    }
                    else
                    {
                        _leverPuzzleHintUnit[i].LiarSign.SetActive(false);
                    }
                }
            }
            _leverPuzzleHintUnit[2].LiarSign.SetActive(false);
            _leverPuzzleHintUnit[2].Lamp.SetActive(true);
        }
    }

    [Serializable]
    public class LeverPuzzleHintUnit
    {
        public GameObject Lamp;
        public GameObject LiarSign;
    }
}