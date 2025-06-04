using System;
using _Script.Manager;
using _Script.Puzzle;
using UnityEngine;
using Zenject;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Door[] _doors;
    private bool[] doorStates;

    [Inject] private PuzzleManager _puzzleManager;
    private bool[] solvedPuzzles = new bool[5];

    [SerializeField] private PuzzleBase[] _puzzleBase;
    private bool[] puzzleBaseStates;

    [Inject] private GameManager _gameManager;
    [SerializeField] private Transform saveZone;
    
    [SerializeField] private Transform spawnPosition;

    [Inject] private MonsterController _monsterController;

    [SerializeField] private bool isFirstSave = true;

    public void Awake()
    {
        doorStates = new bool[_doors.Length];

        puzzleBaseStates = new bool[_puzzleBase.Length];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Save(); 
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Load();
        }
    }

    public void Save()
    {
        isFirstSave = false;
        
        SaveDoor();

        solvedPuzzles = _puzzleManager.SolvedPuzzles;

        for (int i = 0; i < _puzzleBase.Length; i++)
        {
            puzzleBaseStates[i] = _puzzleBase[i].IsSolved;
        }
        
        Debug.Log("Сохранено");
    }

    private void SaveDoor()
    {
        
        for (int i = 0; i < _doors.Length; i++)
        {
            doorStates[i] = _doors[i].isOpen;
        }
    }


    public void Load()
    {
        LoadDoor();

        if (_puzzleBase != null)
        {
            _puzzleManager.SilentPuzzleSolved(solvedPuzzles);
        
            for (int i = 0; i < _puzzleBase.Length; i++)
            {
                _puzzleBase[i].IsSolved = puzzleBaseStates[i];
            }
        }

        if (isFirstSave)
        {
            TeleportPlayer(spawnPosition.position);
        }
        else
        {
            TeleportPlayer(saveZone.position);
        }
        
        _monsterController._navMeshAgent.gameObject.SetActive(false);
        _monsterController.ChangeState(MonsterState.SitsInAHole);

        Debug.Log("Загружено");
    }

    private void TeleportPlayer(Vector3 pos)
    {
        _gameManager.Player.GetComponent<CharacterController>().enabled = false;
        _gameManager.Player.transform.position = pos;
        _gameManager.Player.GetComponent<CharacterController>().enabled = true;
    }

    private void LoadDoor()
    {
        if(_doors == null)
            return;
        
        for (int i = 0; i < doorStates.Length; i++)
        {
            if (doorStates[i])
            {
                _doors[i].SilentOpenDoor();
            }
            else
            {
                _doors[i].CloseDoor();
            }
        }
    }
}
