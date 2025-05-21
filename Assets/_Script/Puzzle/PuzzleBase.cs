using System;
using _Script.Puzzle;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public abstract class PuzzleBase : SerializedMonoBehaviour, IPuzzleStatus
{
    [Inject] private PuzzleManager _puzzleManager;
    
    [SerializeField] private int PuzzleId;
    
    protected bool IsSolved = false;
    
    public event Action PuzzleSolved;

    protected void OnPuzzleSolved()
    {
        PuzzleSolved?.Invoke();
        _puzzleManager.PuzzleSoled(PuzzleId);
    }
}

public interface IPuzzleStatus
{
    public event Action PuzzleSolved;
}
