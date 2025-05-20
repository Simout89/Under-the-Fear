using System;
using _Script.Puzzle;
using UnityEngine;
using Zenject;

public class Door : MonoBehaviour
{
    [Inject] private PuzzleManager _puzzleManager;
    [Header("Settings")]
    [SerializeField] private float _openYRotation;
    [SerializeField] private int puzzleId;

    [Header("References")]
    [SerializeField] private Transform doorPivot;

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
        var rotation = transform.rotation.eulerAngles;
        
        if(puzzleId == obj)
            doorPivot.rotation = Quaternion.Euler(rotation.x, _openYRotation, rotation.z);
    }
}
