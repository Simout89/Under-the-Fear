using System;
using UnityEngine;

namespace _Script.Puzzle
{
    public class PuzzleManager: MonoBehaviour
    {
        public event Action<int> OnPuzzleSolved;
        public bool[] SolvedPuzzles { get; private set; } = new bool[5];

        public void PuzzleSoled(int n)
        {
            SolvedPuzzles[n] = true;
            
            Debug.Log($"PuzzleManager: головоломка {n} решена");
            
            OnPuzzleSolved?.Invoke(n);
        }
    }
}