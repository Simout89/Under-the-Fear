using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class UiManager : MonoBehaviour
{
    [Inject] private GameStateManager _gameStateManager;
    
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject playerMenu;

    public void ShowPoint() => point.SetActive(true);
    public void HidePoint() => point.SetActive(false);

    private void OnEnable()
    {
        _gameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }
    private void OnDisable()
    {
        _gameStateManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState obj)
    {
        if (obj == GameState.Menu)
        {
            playerMenu.SetActive(true);
        }else if (obj == GameState.Play)
        {
            playerMenu.SetActive(false);
        }
    }
}
