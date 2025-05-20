using System;
using _Script.Puzzle;
using UnityEngine;
using Zenject;

public class Door : MonoBehaviour
{
    [Inject] private PuzzleManager _puzzleManager;
    [Header("Settings")]
    [SerializeField] private Vector3 _openRotation;
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
        var rotation = transform.localRotation.eulerAngles;
        
        if(puzzleId == obj)
            doorPivot.localRotation = Quaternion.Euler(rotation.x + _openRotation.x, rotation.y + _openRotation.y, rotation.z + _openRotation.z);
    }
}
