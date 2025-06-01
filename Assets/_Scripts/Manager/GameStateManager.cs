using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public bool PlayerMove { private set; get; }
    
    public event Action<GameState> OnGameStateChanged;
    
    private GameState _currentState = GameState.MainMenu;
    private GameState _previousState;
    public GameState CurrentState => _currentState;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
            ChangeState(GameState.MainMenu);
        else
            ChangeState(GameState.Play);
    }


    public void ChangeState(GameState newState)
    {
        if (_currentState == newState) return;

        switch (newState)
        {
            case GameState.MainMenu:
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 1f;
                PlayerMove = false;
                break;
            case GameState.Menu:
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
                PlayerMove = false;
                break;
            case GameState.Play:
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
                PlayerMove = true;
                break;
            case GameState.Die:
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
                PlayerMove = false;
                break;
        }

        _previousState = _currentState;
        
        _currentState = newState;
        
        Debug.Log(CurrentState);
        
        OnGameStateChanged?.Invoke(CurrentState);
    }
}
public enum GameState
{
    MainMenu,
    Menu,
    Play,
    Die
}
