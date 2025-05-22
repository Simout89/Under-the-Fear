using System;
using _Script.Puzzle;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public abstract class PuzzleBase : SerializedMonoBehaviour, IPuzzleStatus
{
    [SerializeField] protected PuzzleSolutionBase _puzzleSolution;
    
    [Inject] private PuzzleManager _puzzleManager;
    
    [SerializeField] private int PuzzleId;
    
    protected bool IsSolved = false;
    
    public event Action PuzzleSolved;
    
    public virtual void Restart()
    {
        _puzzleSolution.Restart();
    }

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
