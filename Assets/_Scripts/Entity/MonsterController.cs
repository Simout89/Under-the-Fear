using System;
using System.Collections;
using System.Collections.Generic;
using _Script.Manager;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class MonsterController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timeWaitOnPoint;
    [SerializeField] private float waitBeforeHaunting;
    [SerializeField] private float hauntingSpeedMovement;
    [SerializeField] private float loseTargetDelay = 5f;
    [SerializeField] private float randomSoundDelay = 60f;
    [SerializeField] private int chanceRandomPlay = 3;
    [SerializeField] private MonsterEyes _monsterEyes;
    private float originSpeed;
    [Header("References")]
    [SerializeField] private List<Hole> holes;
    [SerializeField] public NavMeshAgent _navMeshAgent;
    [Header("Sounds")]
    [SerializeField] private AK.Wwise.Event beforeHauntingSound;
    [SerializeField] private AK.Wwise.Event screamSound;
    [SerializeField] private AK.Wwise.Event step;
    [SerializeField] private AK.Wwise.Event lowRoar;
    [Header("SoundsSettings")]
    private TimedInvoker stepSoundInvoker;
    [SerializeField] private float footstepInterval;
    [SerializeField] private float runFootstepInterval;
    private float _currentFootStepInterval;
    private Vector3 _lastPointToCheck;
    private Coroutine _lastCoroutine;
    private Hole _lastHole;
    private TimedInvoker _randomVoicePlay;
    
    private MonsterState _currentState = MonsterState.SitsInAHole;
    public MonsterState CurrentState => _currentState;
    public event Action<MonsterState> OnGameStateChanged;
    
    [Inject] private GameManager _gameManager;


    private void Awake()
    {
        originSpeed = _navMeshAgent.speed;

        _currentFootStepInterval = footstepInterval;
        stepSoundInvoker = new TimedInvoker(PlayStepSound, _currentFootStepInterval);

        _randomVoicePlay = new TimedInvoker(HandleRandomVoicePlay, randomSoundDelay);
    }

    private void HandleRandomVoicePlay()
    {
        if (Random.Range(0, chanceRandomPlay) == 0)
        {
            if (_navMeshAgent.gameObject.activeSelf)
            {
                lowRoar.Post(_navMeshAgent.gameObject);
            }
            else
            {
                lowRoar.Post(gameObject);
            }
        }
    }

    private void PlayStepSound()
    {
        step.Post(_navMeshAgent.gameObject);
    }

    private void Update()
    {
        if (_navMeshAgent.velocity.magnitude > 0.1f)
        {
            stepSoundInvoker.Tick();
        }
        
        _randomVoicePlay.Tick();
    }

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
                if (!_navMeshAgent.gameObject.activeSelf) // Вылазит из дыры
                {
                    _navMeshAgent.gameObject.SetActive(true);

                    var nearestHole = FindNearestHole(transform);
                    
                    Vector3 direction = nearestHole.transform.forward;
                    
                    _navMeshAgent.Warp(nearestHole.transform.position + direction*-2);

                    _navMeshAgent.gameObject.transform.localRotation = Quaternion.LookRotation(-direction);
                    
                    screamSound.Post(_navMeshAgent.gameObject);
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
        stepSoundInvoker.SetInterval(runFootstepInterval);
        
        _navMeshAgent.isStopped = true;
        beforeHauntingSound.Post(_navMeshAgent.gameObject);
        yield return new WaitForSeconds(waitBeforeHaunting);
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = hauntingSpeedMovement;
   
        float loseTargetDelay = 0f;
   
        while (Vector3.Distance(_navMeshAgent.transform.position, _gameManager.Player.transform.position) > 1f)
        {
            _navMeshAgent.destination = _gameManager.Player.transform.position;
       
            if (_monsterEyes.SeePlayer)
            {
                loseTargetDelay = 0f;
            }
            else
            {
                loseTargetDelay += Time.deltaTime;
           
                if (loseTargetDelay >= this.loseTargetDelay)
                {
                    _navMeshAgent.speed = originSpeed;
                    ChangeState(MonsterState.BackToTheHole);
                    stepSoundInvoker.SetInterval(footstepInterval);
                    yield break;
                }
            }
       
            yield return null; 
        }
   
        stepSoundInvoker.SetInterval(footstepInterval);
        yield return new WaitForSeconds(timeWaitOnPoint);
        _navMeshAgent.speed = originSpeed;
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
