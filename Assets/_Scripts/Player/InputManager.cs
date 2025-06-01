using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Script.Player
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActionAsset;
    
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
    
        private void OnEnable()
        {
            _inputActionAsset.FindAction("OpenMenu").performed += OpenMenuHandler;
        }
        private void OnDisable()
        {
            _inputActionAsset.FindAction("OpenMenu").performed -= OpenMenuHandler;
        }

        private void OpenMenuHandler(InputAction.CallbackContext obj)
        {
            if(_gameStateManager.CurrentState == GameState.Die)
                return;
            
            if (_gameStateManager.CurrentState == GameState.Menu || _gameStateManager.CurrentState == GameState.MainMenu)
            {
                _gameStateManager.ChangeState(GameState.Play);
            }
            else
            {
                _gameStateManager.ChangeState(GameState.Menu);
            }
        }
    }
}
