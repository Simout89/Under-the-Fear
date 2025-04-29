using System;
using UnityEngine;

public class MonsterEyes : MonoBehaviour
{
    [SerializeField] private MonsterController _monsterController;
    [SerializeField] private MonsterAi _monsterAi;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // _monsterController.ChangeState(MonsterState.Hunting);
            _monsterAi.Sink(-1,-1,1);
        }
    }
}
