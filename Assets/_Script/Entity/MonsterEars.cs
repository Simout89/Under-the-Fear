using UnityEngine;
using UnityEngine.Serialization;

public class MonsterEars : MonoBehaviour
{ 
    [SerializeField] private int soundCheckStrength = 3;
    [SerializeField] private MonsterController _monsterController;
    
    public void Ears(Vector3 transform, int soundStrength)
    {
        Debug.Log($"Монстер услышал звук: {soundStrength} силы");
        if (_monsterController.CurrentState is MonsterState.SitsInAHole or MonsterState.SoundCheck or MonsterState.BackToTheHole && soundStrength == soundCheckStrength)
        {
            _monsterController.ChangeState(MonsterState.SoundCheck, transform);
        }
    }
}
