using System;
using _Script.Manager;
using _Script.Player;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class UiManager : MonoBehaviour
{
    [Inject] private GameStateManager _gameStateManager;
    [Inject] private GameManager _gameManager;
    private IInput _input => _gameManager.Player.GetComponent<PlayerController>().input;
    
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject playerMenu;

    [SerializeField] private GameObject mapMenu;

    public void ShowPoint() => point.SetActive(true);
    public void HidePoint() => point.SetActive(false);

    private void OnEnable()
    {
        _gameStateManager.OnGameStateChanged += HandleGameStateChanged;
        _input.TabPressed += HandleTabPressed;
    }
    private void OnDisable()
    {
        _gameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        _input.TabPressed -= HandleTabPressed;
    }

    private void HandleTabPressed()
    {
        if(_gameStateManager.CurrentState != GameState.Play)
            return;
        
        if (mapMenu.activeSelf)
        {
            mapMenu.SetActive(false);
        }
        else
        {
            mapMenu.SetActive(true);
        }
    }

    private void HandleGameStateChanged(GameState obj)
    {
        if (obj == GameState.Menu)
        {
            playerMenu.SetActive(true);
            mapMenu.SetActive(false);
        }else if (obj == GameState.Play)
        {
            playerMenu.SetActive(false);
        }
    }
}
