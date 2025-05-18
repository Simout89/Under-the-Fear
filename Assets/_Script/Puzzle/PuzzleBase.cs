using System;
using _Script.Puzzle;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PuzzleBase : SerializedMonoBehaviour, IPuzzleStatus
{
    protected bool IsSolved = false;
    
    public event Action PuzzleSolved;

    protected void OnPuzzleSolved()
    {
        PuzzleSolved?.Invoke();
    }
}

public interface IPuzzleStatus
{
    public event Action PuzzleSolved;
}
