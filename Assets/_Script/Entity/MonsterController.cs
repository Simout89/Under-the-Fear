using System;
using System.Collections;
using System.Collections.Generic;
using _Script.Manager;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private float timeWaitOnPoint;
    [SerializeField] private List<Hole> holes;
    [SerializeField] public NavMeshAgent _navMeshAgent;
    private Vector3 _lastPointToCheck;
    private Coroutine _lastCoroutine;
    private Hole _lastHole;
    
    private MonsterState _currentState = MonsterState.SitsInAHole;
    public MonsterState CurrentState => _currentState;
    public event Action<MonsterState> OnGameStateChanged;
    
    [Inject] private GameManager _gameManager;

    // TODO: сделать имитацию слуха
    
    public void ChangeState(MonsterState newState, Vector3 transform = new Vector3())
    {
        if(newState != MonsterState.SoundCheck && CurrentState == MonsterState.SoundCheck)
            if (_currentState == newState) return;

        switch (newState)
        {
            case MonsterState.SitsInAHole:
            {
                if(_lastCoroutine != null)
                    StopCoroutine(_lastCoroutine);
                _navMeshAgent.gameObject.SetActive(false);
            }break;
            case MonsterState.SoundCheck:
            {
                if (!_navMeshAgent.gameObject.activeSelf)
                {
                    _navMeshAgent.gameObject.SetActive(true);
                    _navMeshAgent.gameObject.transform.position = FindNearestHole(transform).transform.position;
                }
                _navMeshAgent.destination = transform;
                _lastPointToCheck = transform;
                if(_lastCoroutine != null)
                    StopCoroutine(_lastCoroutine);
                _lastCoroutine = StartCoroutine(WaitOnPoint(transform, MonsterState.BackToTheHole));
            }break;
            case MonsterState.BackToTheHole:
            {
                Vector3 nearestHole = FindNearestHole(_navMeshAgent.transform.position).transform.position;
                _navMeshAgent.destination = nearestHole;
                if(_lastCoroutine != null)
                    StopCoroutine(_lastCoroutine);
                _lastCoroutine = StartCoroutine(WaitOnPoint(nearestHole, MonsterState.SitsInAHole));
            }break;
            case MonsterState.Hunting:
            {
                if(_lastCoroutine != null)
                    StopCoroutine(_lastCoroutine);
                _lastCoroutine = StartCoroutine(Haunting());
            }break;
        }
        
        _currentState = newState;
        
        Debug.Log(CurrentState);
        
        OnGameStateChanged?.Invoke(CurrentState);
    }


    private IEnumerator Haunting()
    {
        while (Vector3.Distance(_navMeshAgent.transform.position, _gameManager.Player.transform.position) > 1f)
        {
            _navMeshAgent.destination = _gameManager.Player.transform.position;
            yield return null; 
        }
        yield return new WaitForSeconds(timeWaitOnPoint);
        ChangeState(MonsterState.BackToTheHole);
    }

    private IEnumerator WaitOnPoint(Vector3 pointPosition, MonsterState monsterState)
    {
        while (Vector3.Distance(_navMeshAgent.transform.position, pointPosition) > 2f)
        {
            yield return null; 
        }
        yield return new WaitForSeconds(timeWaitOnPoint);
        ChangeState(monsterState);
    }
    
    private Hole FindNearestHole(Vector3 soundPosition)
    {
        Hole nearestHole = holes[0];
        foreach (var hole in holes)
        {
            if (Vector3.Distance(nearestHole.transform.position, soundPosition) >
                Vector3.Distance(hole.transform.position, soundPosition))
            {
                nearestHole = hole;
            }
        }
        return nearestHole;
    }
}

public enum MonsterState
{
    SitsInAHole,
    SoundCheck,
    BackToTheHole,
    Hunting
}
