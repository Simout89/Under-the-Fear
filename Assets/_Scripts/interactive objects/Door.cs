using System;
using System.Collections;
using _Script.Puzzle;
using UnityEngine;
using Zenject;

public class Door : MonoBehaviour
{
    [Inject] private PuzzleManager _puzzleManager;
    [Header("Settings")]
    [SerializeField] private Vector3 _openRotation;
    [SerializeField] private int puzzleId;
    [SerializeField] private float openDelay;
    private Vector3 originRotation;
    public bool isOpen { get; private set; }

    [Header("References")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private AK.Wwise.Event openSound;

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
        if(puzzleId == obj)
        {
            OpenDoor();
        }
    }

    private void Awake()
    {
        originRotation = transform.localRotation.eulerAngles;
    }

    private IEnumerator Delay(bool silent)
    {
        var rotation = originRotation;
        
        doorPivot.localRotation = Quaternion.Euler(rotation.x + _openRotation.x, rotation.y + _openRotation.y,
            rotation.z + _openRotation.z);
        
        isOpen = true;

        yield return new WaitForSeconds(openDelay);
        
        if (openSound != null && !silent)
        {
            openSound.Post(gameObject);
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(Delay(false));
    }

    public void SilentOpenDoor()
    {
        StartCoroutine(Delay(true));
    }

    public void CloseDoor()
    {
        doorPivot.localRotation = Quaternion.Euler(originRotation);
        isOpen = false;
    }
}
