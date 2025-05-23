using System;
using _Script.Manager;
using _Script.Player;
using UnityEngine;
using Zenject;

public class GamePauseController : MonoBehaviour
{
    [Inject] private GameManager _gameManager;
    [Inject] private GameStateManager _gameStateManager;
    private IInput _input => _gameManager.Player.GetComponent<PlayerController>().input;

    private void OnEnable()
    {
        _input.EscPressed += HandleEscPressed;
    }
    private void OnDisable()
    {
        _input.EscPressed -= HandleEscPressed;
    }

    private void HandleEscPressed()
    {
        Debug.Log(13124);
        if (_gameStateManager.CurrentState == GameState.Play)
        {
            _gameStateManager.ChangeState(GameState.Menu);
        }else
        if (_gameStateManager.CurrentState == GameState.Menu)
        {
            _gameStateManager.ChangeState(GameState.Play);
        }
    }
}
