using System;
using System.Collections;
using _Script.Puzzle;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public abstract class PuzzleBase : SerializedMonoBehaviour, IPuzzleStatus
{
    [Inject] private PuzzleManager _puzzleManager;
    [Header("Settings")]
    [SerializeField] private int PuzzleId;
    [SerializeField] private float sirenDuration;
    [Header("References")]
    [SerializeField] protected PuzzleSolutionBase _puzzleSolution;

    [Header("Sound")]
    [SerializeField] private AK.Wwise.Event successSound;
    [SerializeField] private AK.Wwise.Event failSoundStart;
    [SerializeField] private AK.Wwise.Event failSoundStop;
    [SerializeField] private AK.Wwise.Event failSound;
    
    protected bool IsSolved = false;

    private bool _canPlaySiren = true;
    
    public event Action PuzzleSolved;
    
    public virtual void Restart()
    {
        _puzzleSolution.Restart();
    }

    protected void OnPuzzleSolved()
    {
        if(IsSolved)
            return;
        if(!_canPlaySiren)
            return;
        PuzzleSolved?.Invoke();
        _puzzleManager.PuzzleSoled(PuzzleId);
        successSound.Post(gameObject);
        IsSolved = true;
    }

    protected void Fail()
    {
        if(!_canPlaySiren)
            return;
        StartCoroutine(PlaySiren());
    }

    private IEnumerator PlaySiren()
    {
        _canPlaySiren = false;
        
        failSound.Post(gameObject);
        failSoundStart.Post(gameObject);
        yield return new WaitForSeconds(sirenDuration);
        failSoundStop.Post(gameObject);    

        _canPlaySiren = true;
    }
}

public interface IPuzzleStatus
{
    public event Action PuzzleSolved;
}
