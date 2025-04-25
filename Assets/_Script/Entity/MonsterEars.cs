using UnityEngine;

public class MonsterEars : MonoBehaviour
{
    [SerializeField] private MonsterController _monsterController;
    
    public void Ears(Vector3 transform, int soundStrength)
    {
        if (_monsterController.CurrentState is MonsterState.SitsInAHole or MonsterState.SoundCheck or MonsterState.BackToTheHole)
        {
            _monsterController.ChangeState(MonsterState.SoundCheck, transform);
        }
    }
}
