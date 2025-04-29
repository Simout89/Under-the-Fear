using System;
using _Script.Manager;
using _Script.Player;
using UnityEngine;
using Zenject;

public class MonsterEyes : MonoBehaviour
{
    [Inject] private GameManager _gameManager;
    
    [SerializeField] private MonsterController _monsterController;
    [SerializeField] private MonsterAi _monsterAi;
    [SerializeField] private Transform rayCast;
    private bool raySeePlayer = false;
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && raySeePlayer)
        {
            _monsterAi.Sink(-1,-1,1);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayCast.position, _gameManager.Player.transform.position - rayCast.position, out hit, Vector3.Distance(_gameManager.Player.transform.position, rayCast.position), ~0 ,QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.TryGetComponent(out PlayerController playerController))
            {
                raySeePlayer = true;
                Debug.DrawRay(rayCast.position, _gameManager.Player.transform.position - rayCast.position, Color.green); 
            }
            else
            {
                raySeePlayer = false;
                Debug.DrawRay(rayCast.position, _gameManager.Player.transform.position - rayCast.position, Color.red); 

            }
        }   
    }
}
