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
    
    [Header("Teleportation Settings")]
    [SerializeField] private float teleportationTime = 2f;
    [SerializeField] private float pathRecalculationInterval = 3f; // Увеличиваем интервал
    [SerializeField] private float teleportCooldown = 5f; // Кулдаун на телепортацию
    [SerializeField] private float minDistanceForTeleport = 10f; // Минимальная дистанция для телепортации
    
    [Header("References")]
    [SerializeField] private List<Hole> holes;
    [SerializeField] public NavMeshAgent _navMeshAgent;
    
    [Header("Sounds")]
    [SerializeField] private AK.Wwise.Event beforeHauntingSound;
    [SerializeField] private AK.Wwise.Event screamSound;
    [SerializeField] private AK.Wwise.Event step;
    [SerializeField] private AK.Wwise.Event lowRoar;
    
    [Header("Music")]
    [SerializeField] private AK.Wwise.Event hauntingMusic;
    
    [Header("SoundsSettings")]
    private TimedInvoker stepSoundInvoker;
    [SerializeField] private float footstepInterval;
    [SerializeField] private float runFootstepInterval;
    private float _currentFootStepInterval;
    private Vector3 _lastPointToCheck;
    private Coroutine _lastCoroutine;
    private Hole _lastHole;
    private TimedInvoker _randomVoicePlay;
    private float _lastTeleportTime = -999f; // Время последней телепортации
    
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
    }

    // Методы для расчета оптимального пути
    private float CalculatePathTime(Vector3 from, Vector3 to)
    {
        NavMeshPath path = new NavMeshPath();
        
        // Используем статический метод без перемещения агента
        if (!NavMesh.CalculatePath(from, to, _navMeshAgent.areaMask, path) || path.status != NavMeshPathStatus.PathComplete)
        {
            return float.MaxValue;
        }

        float distance = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return distance / _navMeshAgent.speed;
    }

    private (float time, Hole exitHole) CalculateTeleportPathTime(Vector3 from, Vector3 to, Hole entryHole)
    {
        float bestTime = float.MaxValue;
        Hole bestExitHole = null;
        
        foreach (var exitHole in holes)
        {
            if (exitHole == entryHole) continue;
            
            float timeToEntry = CalculatePathTime(from, entryHole.transform.position);
            float timeFromExit = CalculatePathTime(exitHole.transform.position, to);
            
            if (timeToEntry != float.MaxValue && timeFromExit != float.MaxValue)
            {
                float totalTime = timeToEntry + teleportationTime + timeFromExit;
                
                if (totalTime < bestTime)
                {
                    bestTime = totalTime;
                    bestExitHole = exitHole;
                }
            }
        }
        
        return (bestTime, bestExitHole);
    }

    private (bool useTeleport, Hole entryHole, Hole exitHole) FindOptimalPath(Vector3 from, Vector3 to)
    {
        float directTime = CalculatePathTime(from, to);
        
        // Проверяем кулдаун телепортации
        if (Time.time - _lastTeleportTime < teleportCooldown)
        {
            return (false, null, null);
        }
        
        // Проверяем минимальную дистанцию
        if (Vector3.Distance(from, to) < minDistanceForTeleport)
        {
            return (false, null, null);
        }
        
        float bestTeleportTime = float.MaxValue;
        Hole bestEntryHole = null;
        Hole bestExitHole = null;
        
        foreach (var entryHole in holes)
        {
            var (teleportTime, exitHole) = CalculateTeleportPathTime(from, to, entryHole);
            
            if (teleportTime < bestTeleportTime)
            {
                bestTeleportTime = teleportTime;
                bestEntryHole = entryHole;
                bestExitHole = exitHole;
            }
        }
        
        // Телепортация должна быть значительно быстрее (минимум на 30%)
        if (bestTeleportTime < directTime * 0.7f && bestTeleportTime != float.MaxValue)
        {
            return (true, bestEntryHole, bestExitHole);
        }
        
        return (false, null, null);
    }

    private bool IsPositionReachable(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        return _navMeshAgent.CalculatePath(targetPosition, path) && path.status == NavMeshPathStatus.PathComplete;
    }

    private Vector3 FindAlternativePosition(Vector3 originalTarget)
    {
        for (float radius = 1f; radius <= 10f; radius += 1f)
        {
            for (int angle = 0; angle < 360; angle += 45)
            {
                Vector3 direction = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );
                
                Vector3 testPosition = originalTarget + direction * radius;
                
                if (NavMesh.SamplePosition(testPosition, out NavMeshHit hit, 1f, _navMeshAgent.areaMask))
                {
                    if (IsPositionReachable(hit.position))
                    {
                        return hit.position;
                    }
                }
            }
        }
        
        return _navMeshAgent.transform.position;
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
                if (!_navMeshAgent.gameObject.activeSelf)
                {
                    _navMeshAgent.gameObject.SetActive(true);

                    var nearestHole = FindNearestHole(transform);
                    
                    Vector3 direction = nearestHole.transform.forward;
                    Vector3 spawnPosition = nearestHole.transform.position + direction * -2;
                    
                    // Безопасное появление из щели
                    if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, _navMeshAgent.areaMask))
                    {
                        _navMeshAgent.Warp(hit.position);
                    }
                    else
                    {
                        _navMeshAgent.Warp(nearestHole.transform.position);
                    }
                    
                    _navMeshAgent.gameObject.transform.localRotation = Quaternion.LookRotation(-direction);
                    
                    screamSound.Post(_navMeshAgent.gameObject);
                }

                // Проверяем достижимость цели
                Vector3 targetPosition = transform;
                if (!IsPositionReachable(targetPosition))
                {
                    targetPosition = FindAlternativePosition(targetPosition);
                }

                // Выбираем оптимальный путь
                var (useTeleport, entryHole, exitHole) = FindOptimalPath(_navMeshAgent.transform.position, targetPosition);
                
                if (useTeleport && entryHole != null && exitHole != null)
                {
                    if(_lastCoroutine != null) StopCoroutine(_lastCoroutine);
                    _lastCoroutine = StartCoroutine(TeleportToTarget(entryHole, exitHole, targetPosition));
                }
                else
                {
                    _navMeshAgent.destination = targetPosition;
                    _lastPointToCheck = targetPosition;
                    if(_lastCoroutine != null) StopCoroutine(_lastCoroutine);
                    _lastCoroutine = StartCoroutine(WaitOnPoint(targetPosition, MonsterState.BackToTheHole));
                }
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

    private IEnumerator TeleportToTarget(Hole entryHole, Hole exitHole, Vector3 finalTarget)
    {
        _lastTeleportTime = Time.time; // Записываем время телепортации
        
        // Идем к входной щели
        _navMeshAgent.destination = entryHole.transform.position;
        
        while (Vector3.Distance(_navMeshAgent.transform.position, entryHole.transform.position) > 2f)
        {
            yield return null;
        }
        
        // Прячемся в щель
        _navMeshAgent.gameObject.SetActive(false);
        yield return new WaitForSeconds(teleportationTime);
        
        // Появляемся из выходной щели
        _navMeshAgent.gameObject.SetActive(true);
        
        // Безопасная телепортация с проверкой NavMesh
        Vector3 exitDirection = exitHole.transform.forward;
        Vector3 spawnPosition = exitHole.transform.position + exitDirection * -2;
        
        // Проверяем и корректируем позицию на NavMesh
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, _navMeshAgent.areaMask))
        {
            _navMeshAgent.Warp(hit.position);
        }
        else
        {
            // Если не можем найти валидную позицию, используем позицию щели
            _navMeshAgent.Warp(exitHole.transform.position);
        }
        
        _navMeshAgent.gameObject.transform.localRotation = Quaternion.LookRotation(-exitDirection);
        
        screamSound.Post(_navMeshAgent.gameObject);
        
        // Идем к финальной цели
        _navMeshAgent.destination = finalTarget;
        _lastPointToCheck = finalTarget;
        
        yield return StartCoroutine(WaitOnPoint(finalTarget, MonsterState.BackToTheHole));
    }

    private IEnumerator QuickTeleportToPlayer(Hole entryHole, Hole exitHole)
    {
        _lastTeleportTime = Time.time; // Записываем время телепортации
        
        // Быстрая телепортация во время охоты
        _navMeshAgent.destination = entryHole.transform.position;
        
        while (Vector3.Distance(_navMeshAgent.transform.position, entryHole.transform.position) > 2f)
        {
            yield return null;
        }
        
        _navMeshAgent.gameObject.SetActive(false);
        yield return new WaitForSeconds(teleportationTime * 0.5f); // Быстрее во время охоты
        
        _navMeshAgent.gameObject.SetActive(true);
        
        // Безопасная телепортация с проверкой NavMesh
        Vector3 exitDirection = exitHole.transform.forward;
        Vector3 spawnPosition = exitHole.transform.position + exitDirection * -2;
        
        // Проверяем и корректируем позицию на NavMesh
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, _navMeshAgent.areaMask))
        {
            _navMeshAgent.Warp(hit.position);
        }
        else
        {
            // Если не можем найти валидную позицию, используем позицию щели
            _navMeshAgent.Warp(exitHole.transform.position);
        }
        
        _navMeshAgent.gameObject.transform.localRotation = Quaternion.LookRotation(-exitDirection);
        
        screamSound.Post(_navMeshAgent.gameObject);
    }

    private IEnumerator Haunting()
    {
        stepSoundInvoker.SetInterval(runFootstepInterval);
        
        _navMeshAgent.isStopped = true;
        beforeHauntingSound.Post(_navMeshAgent.gameObject);
        yield return new WaitForSeconds(waitBeforeHaunting);
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = hauntingSpeedMovement;
        
        AkUnitySoundEngine.SetSwitch("DangerTheme", "Main_Loop", gameObject);
        hauntingMusic.Post(gameObject);
   
        float loseTargetTimer = 0f;
        float pathRecalculationTimer = 0f;
        bool hasKilledPlayer = false; // Флаг для предотвращения спама убийства
   
        while (Vector3.Distance(_navMeshAgent.transform.position, _gameManager.Player.transform.position) > 1f)
        {
            Vector3 playerPosition = _gameManager.Player.transform.position;
            
            // Проверяем достижимость позиции игрока
            if (!IsPositionReachable(playerPosition))
            {
                playerPosition = FindAlternativePosition(playerPosition);
            }
            
            pathRecalculationTimer += Time.deltaTime;
            
            // Периодически пересчитываем оптимальный путь (реже и только если монстр не движется активно)
            if (pathRecalculationTimer >= pathRecalculationInterval && _navMeshAgent.velocity.magnitude < 1f)
            {
                var (useTeleport, entryHole, exitHole) = FindOptimalPath(
                    _navMeshAgent.transform.position, 
                    playerPosition
                );
                
                if (useTeleport && entryHole != null && exitHole != null)
                {
                    yield return StartCoroutine(QuickTeleportToPlayer(entryHole, exitHole));
                }
                
                pathRecalculationTimer = 0f;
            }
            
            // Обычное движение к игроку
            _navMeshAgent.destination = playerPosition;
       
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
                    stepSoundInvoker.SetInterval(footstepInterval);
                    AkUnitySoundEngine.SetSwitch("DangerTheme", "Live_End", gameObject);
                    hauntingMusic.Post(gameObject);
                    ChangeState(MonsterState.BackToTheHole);

                    yield break;
                }
            }
       
            yield return null; 
        }
        
        // Убийство игрока происходит только один раз
        if (!hasKilledPlayer)
        {
            hasKilledPlayer = true;
            
            AkUnitySoundEngine.SetSwitch("DangerTheme", "Dead_End", gameObject);
            hauntingMusic.Post(gameObject);

            _playerHealth.TakeDamage(99999);
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