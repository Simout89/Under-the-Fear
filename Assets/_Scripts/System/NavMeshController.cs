using System;
using _Script.Puzzle;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

public class NavMeshController : MonoBehaviour
{
    [Inject] private PuzzleManager _puzzleManager;

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
        Bake();
    }

    [SerializeField] private NavMeshSurface _navMeshSurface;
    private void Awake()
    {
        Bake();
    }

    public void Bake()
    {
        _navMeshSurface.BuildNavMesh();
    }
}
