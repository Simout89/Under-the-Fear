using UnityEngine;
using UnityEngine.Serialization;

public class MonsterEars : MonoBehaviour
{ 
    [SerializeField] private int soundCheckStrength = 3;
    [SerializeField] private MonsterController _monsterController;
    [SerializeField] private MonsterAi _monsterAi;
    
    public void Ears(Vector3 transform, int soundStrength)
    {
        Debug.Log($"Монстер услышал звук: {soundStrength} силы");
        if (_monsterController.CurrentState is MonsterState.SitsInAHole or MonsterState.SoundCheck or MonsterState.BackToTheHole && soundStrength == soundCheckStrength)
        {
            // _monsterController.ChangeState(MonsterState.SoundCheck, transform);
        }

        if (_monsterController.CurrentState == MonsterState.SitsInAHole)
        {
            _monsterAi.Sink(-1, soundStrength, 0, transform);
        }
        
        if (_monsterController.CurrentState is MonsterState.SoundCheck or MonsterState.BackToTheHole)
        {
            _monsterAi.Sink(Vector3.Distance(transform, _monsterController._navMeshAgent.transform.position), soundStrength, 0, transform);
        }
    }
}
