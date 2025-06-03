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
    [SerializeField] private float pathCheckRadius = 5f; // Радиус для поиска доступной точки
    [SerializeField] private float stuckCheckTime = 3f; // Время проверки застревания
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
    
    // Для отслеживания застревания
    private Vector3 _lastPosition;
    private float _stuckTimer;
    private bool _isStuck;
    
    private MonsterState _currentState = MonsterState.SitsInAHole;
    public MonsterState CurrentState => _currentState;
    public event Action<MonsterState> OnGameStateChanged;
    
    [Inject] private GameManager _gameManager;
    [Inject] private PlayerHealth _playerHealth;

    private void Awake()
    {
        originSpeed = _navMeshAgent.speed;
        _currentFootStepInterval = footstepInterval;
        stepSoundInvoker = new TimedInvoker(PlayStepSound, _currentFootStepInterval);
        _randomVoicePlay = new TimedInvoker(HandleRandomVoicePlay, randomSoundDelay);
        _lastPosition = transform.position;
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
                lowRoar.Post(FindNearestHole(_gameManager.Player.transform.position).gameObject);
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
        
        // Проверяем застревание
        CheckIfStuck();
    }
    
    private void CheckIfStuck()
    {
        if (!_navMeshAgent.gameObject.activeSelf || _currentState == MonsterState.SitsInAHole)
            return;
            
        float distanceMoved = Vector3.Distance(transform.position, _lastPosition);
        
        // Если монстр почти не двигается, но должен идти
        if (distanceMoved < 0.1f && _navMeshAgent.hasPath && !_navMeshAgent.isStopped)
        {
            _stuckTimer += Time.deltaTime;
            
            if (_stuckTimer >= stuckCheckTime && !_isStuck)
            {
                _isStuck = true;
                Debug.LogWarning("[MonsterController] Монстр застрял! Пытаемся исправить...");
                HandleStuckSituation();
            }
        }
        else
        {
            _stuckTimer = 0f;
            _isStuck = false;
        }
        
        _lastPosition = transform.position;
    }
    
    private void HandleStuckSituation()
    {
        switch (_currentState)
        {
            case MonsterState.SoundCheck:
                // Пытаемся найти ближайшую доступную точку к цели
                Vector3 validPoint = FindValidNavMeshPoint(_lastPointToCheck);
                if (validPoint != Vector3.zero)
                {
                    SetDestinationSafely(validPoint);
                }
                else
                {
                    // Если не удалось найти путь к цели, возвращаемся в дыру
                    ChangeState(MonsterState.BackToTheHole);
                }
                break;
                
            case MonsterState.BackToTheHole:
                // Принудительно телепортируемся в ближайшую дыру
                Hole nearestHole = FindNearestHole(transform.position);
                TeleportToHole(nearestHole);
                ChangeState(MonsterState.SitsInAHole);
                break;
                
            case MonsterState.Hunting:
                // При охоте пытаемся найти путь к игроку или отступаем
                Vector3 playerValidPoint = FindValidNavMeshPoint(_gameManager.Player.transform.position);
                if (playerValidPoint != Vector3.zero)
                {
                    SetDestinationSafely(playerValidPoint);
                }
                else
                {
                    ChangeState(MonsterState.BackToTheHole);
                }
                break;
        }
        
        _isStuck = false;
        _stuckTimer = 0f;
    }
    
    private Vector3 FindValidNavMeshPoint(Vector3 targetPosition)
    {
        NavMeshHit hit;
        
        // Сначала проверяем саму целевую позицию
        if (NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, new NavMeshPath()))
            {
                return hit.position;
            }
        }
        
        // Если прямой путь недоступен, ищем в радиусе
        for (int i = 0; i < 8; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * pathCheckRadius;
            randomDirection += targetPosition;
            
            if (NavMesh.SamplePosition(randomDirection, out hit, pathCheckRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        return hit.position;
                    }
                }
            }
        }
        
        return Vector3.zero; // Не найден доступный путь
    }
    
    private bool SetDestinationSafely(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    _navMeshAgent.SetPath(path);
                    return true;
                }
            }
        }
        
        Debug.LogWarning($"[MonsterController] Не удалось найти путь к {targetPosition}");
        return false;
    }
    
    private void TeleportToHole(Hole hole)
    {
        Vector3 direction = hole.transform.forward;
        Vector3 teleportPosition = hole.transform.position + direction * -2;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(teleportPosition, out hit, 5f, NavMesh.AllAreas))
        {
            _navMeshAgent.Warp(hit.position);
        }
        else
        {
            _navMeshAgent.Warp(hole.transform.position);
        }
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
                _isStuck = false;
                _stuckTimer = 0f;
            }break;
            
            case MonsterState.SoundCheck:
            {
                if (!_navMeshAgent.gameObject.activeSelf)
                {
                    _navMeshAgent.gameObject.SetActive(true);
                    var nearestHole = FindNearestHole(transform);
                    TeleportToHole(nearestHole);
                    screamSound.Post(_navMeshAgent.gameObject);
                }
                
                // Безопасно устанавливаем пункт назначения
                Vector3 validTarget = FindValidNavMeshPoint(transform);
                if (validTarget != Vector3.zero)
                {
                    SetDestinationSafely(validTarget);
                    _lastPointToCheck = validTarget;
                    
                    if(_lastCoroutine != null)
                        StopCoroutine(_lastCoroutine);
                    _lastCoroutine = StartCoroutine(WaitOnPoint(validTarget, MonsterState.BackToTheHole));
                }
                else
                {
                    // Если не можем добраться до цели, сразу возвращаемся
                    ChangeState(MonsterState.BackToTheHole);
                    return;
                }
            }break;
            
            case MonsterState.BackToTheHole:
            {
                Hole nearestHole = FindNearestHole(_navMeshAgent.transform.position);
                Vector3 holePosition = nearestHole.transform.position;
                
                if (SetDestinationSafely(holePosition))
                {
                    if(_lastCoroutine != null)
                        StopCoroutine(_lastCoroutine);
                    _lastCoroutine = StartCoroutine(WaitOnPoint(holePosition, MonsterState.SitsInAHole));
                }
                else
                {
                    // Если не можем добраться до дыры, телепортируемся
                    TeleportToHole(nearestHole);
                    ChangeState(MonsterState.SitsInAHole);
                    return;
                }
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
   
        float loseTargetTimer = 0f;
        float lastUpdateTime = Time.time;
   
        while (Vector3.Distance(_navMeshAgent.transform.position, _gameManager.Player.transform.position) > 1f)
        {
            // Безопасно обновляем цель каждые 0.5 секунд
            if (Time.time - lastUpdateTime > 0.5f)
            {
                Vector3 playerPosition = _gameManager.Player.transform.position;
                Vector3 validPlayerPosition = FindValidNavMeshPoint(playerPosition);
                
                if (validPlayerPosition != Vector3.zero)
                {
                    SetDestinationSafely(validPlayerPosition);
                }
                
                lastUpdateTime = Time.time;
            }
       
            if (_monsterEyes.SeePlayer)
            {
                loseTargetTimer = 0f;
            }
            else
            {
                loseTargetTimer += Time.deltaTime;
           
                if (loseTargetTimer >= this.loseTargetDelay)
                {
                    _navMeshAgent.speed = originSpeed;
                    ChangeState(MonsterState.BackToTheHole);
                    stepSoundInvoker.SetInterval(footstepInterval);
                    yield break;
                }
            }
       
            yield return null; 
        }
        
        _playerHealth.TakeDamage(99999);
   
        stepSoundInvoker.SetInterval(footstepInterval);
        yield return new WaitForSeconds(timeWaitOnPoint);
        _navMeshAgent.speed = originSpeed;
        ChangeState(MonsterState.BackToTheHole);
    }

    private IEnumerator WaitOnPoint(Vector3 pointPosition, MonsterState monsterState)
    {
        float waitTimer = 0f;
        
        while (Vector3.Distance(_navMeshAgent.transform.position, pointPosition) > 2f)
        {
            waitTimer += Time.deltaTime;
            
            // Защита от бесконечного ожидания
            if (waitTimer > 30f)
            {
                Debug.LogWarning("[MonsterController] Таймаут ожидания достижения точки");
                break;
            }
            
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