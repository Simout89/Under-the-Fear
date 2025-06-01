using System;
using UnityEngine;
using Zenject;

namespace _Script.Puzzle
{
    public class PuzzleManager: MonoBehaviour
    {
        public event Action<int> OnPuzzleSolved;
        public event Action AllPuzzleSolved;
        public bool[] SolvedPuzzles { get; private set; } = new bool[5];

        public bool PuzzlesSolved { get; private set; }

        public void PuzzleSoled(int n)
        {
            SolvedPuzzles[n] = true;
            
            Debug.Log($"PuzzleManager: головоломка {n} решена");
            
            OnPuzzleSolved?.Invoke(n);

            if (!PuzzlesSolved)
            {
                foreach (var solvedPuzzle in SolvedPuzzles)
                {
                    if(!solvedPuzzle)
                        return;
                }
            
                AllPuzzleSolved?.Invoke();

                PuzzlesSolved = true;
            
                Debug.Log($"Все головоломки решены");
            }
        }
    }
}