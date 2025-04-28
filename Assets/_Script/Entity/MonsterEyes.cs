using System;
using UnityEngine;

public class MonsterEyes : MonoBehaviour
{
    [SerializeField] private MonsterController _monsterController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            _monsterController.ChangeState(MonsterState.Hunting);
    }
}
